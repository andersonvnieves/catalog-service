using br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CreateOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace br.com.fiap.cloudgames.Catalog.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly CreateOrderUseCase _createOrderUseCase;

        public OrderController(CreateOrderUseCase createOrderUseCase)
        {
            _createOrderUseCase = createOrderUseCase;
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrderRequest request)
        {
            var result = await _createOrderUseCase.ExecuteAsync(request);
            return Ok(result);
        }
    }
}
