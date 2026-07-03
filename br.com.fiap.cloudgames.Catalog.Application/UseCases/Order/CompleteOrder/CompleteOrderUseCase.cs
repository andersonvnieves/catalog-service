using System;
using System.Collections.Generic;
using System.Text;
using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CompleteOrder
{
    public class CompleteOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CompleteOrderUseCase(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(Guid orderId)
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = await _orderRepository.GetByIdAsync(orderId);
            if(order == null)
                throw new ApplicationException("Order not found");
            try
            {
                order.OrderHasBeenPaid();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            return;
        }
    }
}
