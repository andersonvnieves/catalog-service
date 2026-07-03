using System;
using System.Collections.Generic;
using System.Text;
using br.com.fiap.cloudgames.Catalog.Application.Events;
using br.com.fiap.cloudgames.Catalog.Application.Publishers;
using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CreateOrder
{
    public class CreateOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderCreatedEventPublisher _orderCreatedEventPublisher;

        public CreateOrderUseCase(IOrderRepository orderRepository, IGameRepository gameRepository, 
            ILibraryRepository libraryRepository, IUnitOfWork unitOfWork, IOrderCreatedEventPublisher orderCreatedEventPublisher)
        {
            _orderRepository = orderRepository;
            _gameRepository = gameRepository;
            _libraryRepository = libraryRepository;
            _unitOfWork = unitOfWork;
            _orderCreatedEventPublisher = orderCreatedEventPublisher;
        }

        public async Task<CreateOrderResponse> ExecuteAsync(CreateOrderRequest request)
        {
             //Check if game exists and fetch the list
             if(request.GameIds.Count == 0)
                 throw new ApplicationException("No games selected");
             
             var gameIds = new List<Guid>();
             try
             {
                 foreach (var gameId in request.GameIds)
                 {
                     gameIds.Add(Guid.Parse(gameId));
                 }
             }
             catch (Exception ex)
             {
                 throw new ApplicationException("Invalid game id");
             }
             var games = await _gameRepository.GetByIdsAsync(gameIds);
             if(games.Count() != gameIds.Count)
                 throw new ApplicationException("One or more games not found");
             
             //Check if user has games already (its all or nothing, one game owned cancel all others)
             var library = await _libraryRepository.GetByIdAsync(request.UserId);
             if(library != null && library.OwnedGames.Any(x => gameIds.Contains(x.GameId)))
                 throw new ApplicationException("User already owns one or more of the selected games");
             
             //Create order and save 
             var orderItems = new List<OrderItem>();
             var items = new Dictionary<string, decimal>();
             foreach (var game in games)
             {
                 items.Add(game.Title, game.Price.PriceValue);
                 orderItems.Add(new OrderItem(game.Id, game.Price));
             }
             var order = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Order.Create(request.UserId, orderItems);
             try
             {
                 await _unitOfWork.BeginTransactionAsync();
                 await _orderRepository.AddAsync(order);
                 await _unitOfWork.CommitAsync();
             }
             catch (Exception ex)
             {
                 await _unitOfWork.RollbackAsync();
                 throw;
             }
             
             //Send payment event (retry logic)
             await _orderCreatedEventPublisher.PublishAsync(new OrderCreatedEvent()
             {
                 EventId = Guid.NewGuid(),
                 OrderId = order.Id,
                 TotalAmount = order.TotalAmount.PriceValue,
                 UserId = request.UserId,
                 Name = "",
                 Email = ""
             });
             
             return new CreateOrderResponse()
             {
                 OrderId = order.Id.ToString(),
                 Items = items,
                 TotalAmount = order.TotalAmount.PriceValue
             };
        }
    }
}
