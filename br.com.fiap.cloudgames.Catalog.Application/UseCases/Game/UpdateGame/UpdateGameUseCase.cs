using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.UpdateGame
{
    public class UpdateGameUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<UpdateGameUseCase> _logger;

        public UpdateGameUseCase(IUnitOfWork unitOfWork, IGameRepository gameRepository, ILogger<UpdateGameUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<UpdateGameResponse> ExecuteAsync(UpdateGameRequest request)
        {
            _logger.LogInformation(
                "Executing {UseCase}. GameId={GameId}, Title={Title}, ReleaseDate={ReleaseDate}",
                nameof(UpdateGameUseCase),
                request.Id,
                request.Title,
                request.ReleaseDate);

            try
            {
                if (!Guid.TryParse(request.Id, out var gameId))
                {
                    _logger.LogWarning("Invalid game id format. GameId={GameId}", request.Id);
                    throw new ApplicationException($"Invalid game id '{request.Id}'");
                }

                if (!Enum.TryParse<AgeRating>(request.AgeRating, true, out var ageRating))
                {
                    _logger.LogWarning("Invalid age rating. AgeRating={AgeRating}, GameId={GameId}", request.AgeRating, request.Id);
                    throw new ApplicationException($"Invalid age rating '{request.AgeRating}'");
                }

                var gameModes = new List<GameModes>();
                foreach (var requestGameMode in request.GameModes)
                {
                    if (!Enum.TryParse<GameModes>(requestGameMode, true, out var gameMode))
                    {
                        _logger.LogWarning("Invalid game mode. GameMode={GameMode}, GameId={GameId}", requestGameMode, request.Id);
                        throw new ApplicationException($"Invalid game mode '{requestGameMode}'");
                    }
                    gameModes.Add(gameMode);
                }

                var publisher = new Publisher(request.Publisher);

                var developers = new List<Developer>();
                foreach (var requestDeveloper in request.Developers)
                {
                    developers.Add(new Developer(requestDeveloper));
                }               

                var game = await _gameRepository.GetByIdAsync(gameId);
                if (game == null)
                {
                    _logger.LogWarning("Game not found. GameId={GameId}", request.Id);
                    throw new ApplicationException($"Game with id '{request.Id}' not found");
                }

                game.UpdateDetails(
                    request.Title,
                    request.Description,
                    request.Story,
                    request.Franchise,
                    request.ReleaseDate,
                    ageRating,
                    gameModes,
                    publisher,
                    developers);

                _gameRepository.Update(game);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Game updated successfully. GameId={GameId}, Title={Title}", game.Id, game.Title);

                return new UpdateGameResponse()
                {
                    Id = game.Id.ToString(),
                    Title = game.Title,
                    Description = game.Description,
                    Story = game.Story,
                    Franchise = game.Franchise,
                    ReleaseDate = game.ReleaseDate,
                    AgeRating = game.AgeRating.ToString(),
                    Developers = game.Developers.Select(developer => developer.Name).ToList(),
                    GameModes = game.GameModes.Select(gameMode => gameMode.ToString()).ToList(),
                    Publisher = publisher.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing {UseCase}. GameId={GameId}", nameof(UpdateGameUseCase), request.Id);
                throw;
            }
        }
    }
}
