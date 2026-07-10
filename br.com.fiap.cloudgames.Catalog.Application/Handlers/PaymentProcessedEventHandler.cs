using br.com.fiap.cloudgames.Catalog.Application.Events;
using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace br.com.fiap.cloudgames.Catalog.Application.Handlers;

public class PaymentProcessedEventHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;
    private ILogger<PaymentProcessedEventHandler> _logger;

    public PaymentProcessedEventHandler(IOrderRepository orderRepository, 
        ILibraryRepository libraryRepository, 
        IGameRepository gameRepository, 
        IUnitOfWork unitOfWork, 
        ILogger<PaymentProcessedEventHandler> logger)
    {
        _orderRepository = orderRepository;
        _libraryRepository = libraryRepository;
        _gameRepository = gameRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task HandleAsync(PaymentProcessedEvent paymentProcessedEvent)
    {        
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var order = await _orderRepository.GetByIdAsync(paymentProcessedEvent.OrderId);
            if (order == null)
                throw new ApplicationException("Order not found");

            if (order.OrderStatus != Domain.Enums.OrderStatus.Pending)
                throw new ApplicationException("Order already processed.");

            switch (paymentProcessedEvent.PaymentStatus)
            {
                case Domain.Enums.PaymentStatus.Approved:
                    await FulfillOrder(order);
                    break;
                case Domain.Enums.PaymentStatus.Rejected:
                    await RejectOrder(order);
                    break;
            }
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }        
    }

    public async Task FulfillOrder(Order order)
    {
        //Complete Order as paid
        order.OrderHasBeenPaid();
        await _orderRepository.UpdateAsync(order);
       

        var gamesIds = order.Items.Select(o => o.GameId).ToList();
        var games = await _gameRepository.GetByIdsAsync(gamesIds);
        if (games.Count() != gamesIds.Count)
            throw new ApplicationException("One or more games not found");

        // Handle Library
        var library = await _libraryRepository.GetByIdAsync(order.UserId);
        if (library == null)
        {
            //Create User Library
            library = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library.Create(order.UserId);
            await _libraryRepository.AddAsync(library);
        }

        //Add Game to User Library
        foreach (var game in games)
        {
            library.AddGame(new OwnedGame(game.Id, order.Id, DateTime.Now));
        }
        await _libraryRepository.UpdateAsync(library);
    }

    public async Task RejectOrder(Order order)
    {
        //Complete Order as cancelled
        order.OrderHasBeenCancelled();
        await _orderRepository.UpdateAsync(order);        
    }
}
