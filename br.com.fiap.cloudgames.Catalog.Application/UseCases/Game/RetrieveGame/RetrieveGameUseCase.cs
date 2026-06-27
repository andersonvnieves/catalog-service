using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.RetrieveGame
{
    public class RetrieveGameUseCase
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<RetrieveGameUseCase> _logger;

        public RetrieveGameUseCase(IGameRepository gameRepository, ILogger<RetrieveGameUseCase> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<RetrieveGameResponse> ExecuteAsync(RetrieveGameRequest request)
        {
            _logger.LogInformation("Executing {UseCase}. GameId={GameId}", nameof(RetrieveGameUseCase), request.GameId);

            try
            {
                var parseResult = Guid.TryParse(request.GameId, out var gameId);
                if (!parseResult)
                {
                    _logger.LogWarning("Invalid game id format. GameId={GameId}", request.GameId);
                    throw new ApplicationException($"Invalid game id '{request.GameId}'");
                }

                var game = await _gameRepository.GetByIdAsync(gameId);
                if (game == null)
                {
                    _logger.LogWarning("Game not found. GameId={GameId}", request.GameId);
                    throw new ApplicationException($"Game with id '{request.GameId}' not found");
                }

                _logger.LogInformation("Game retrieved successfully. GameId={GameId}, Title={Title}", game.Id, game.Title);

                return new RetrieveGameResponse()
                {
                    Id = game.Id.ToString(),
                    Title = game.Title,
                    Description = game.Description,
                    Story = game.Story,
                    Franchise = game.Franchise,
                    ReleaseDate = game.ReleaseDate,
                    AgeRating = game.AgeRating.ToString(),
                    GameModes = game.GameModes.Select(mode => mode.ToString()).ToList(),
                    Publisher = game.Publisher.Name,
                    Developers = game.Developers.Select(developer => developer.Name).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing {UseCase}. GameId={GameId}", nameof(RetrieveGameUseCase), request.GameId);
                throw;
            }
        }
    }
}
