using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.CreateGame;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.RetrieveGame;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.UpdateGame;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace br.com.fiap.cloudgames.Catalog.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly CreateGameUseCase _createGameUseCase;
        private readonly RetrieveGameUseCase _retrieveGameUseCase;
        private readonly UpdateGameUseCase _updateGameUseCase;
        private const string ADMIN_ROLE = "admin";
        public GameController(CreateGameUseCase createGameUseCase,
            RetrieveGameUseCase retrieveGameUseCase,
            UpdateGameUseCase updateGameUseCase)
        {
            _createGameUseCase = createGameUseCase;
            _retrieveGameUseCase = retrieveGameUseCase;
            _updateGameUseCase = updateGameUseCase;
        }

        [Authorize(Roles = ADMIN_ROLE)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateGameRequest request)
        {
            var result = await _createGameUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] String Id)
        {
            var result = await _retrieveGameUseCase.ExecuteAsync(new RetrieveGameRequest() { GameId = Id });
            return Ok(result);
        }

        [Authorize(Roles = ADMIN_ROLE)]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateGameRequest request)
        {
            var result = await _updateGameUseCase.ExecuteAsync(request);
            return Ok(result);
        }
    }
}
