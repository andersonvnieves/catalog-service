using br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CreateOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace br.com.fiap.cloudgames.Catalog.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly CreateOrderUseCase _createOrderUseCase;
        private const string BASIC_ROLE = "user";

        public OrderController(CreateOrderUseCase createOrderUseCase)
        {
            _createOrderUseCase = createOrderUseCase;
        }
        
        [Authorize(Roles = BASIC_ROLE)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrderRequest request)
        {
            var result = await _createOrderUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(Get), new { id = result.OrderId }, result);
        }
        
        [Authorize(Roles = BASIC_ROLE)]
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            throw new NotImplementedException();
        }
    }
}
