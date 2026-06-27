using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.CreateGame
{
    public class CreateGameUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<CreateGameUseCase> _logger;

        public CreateGameUseCase(IUnitOfWork unitOfWork, IGameRepository gameRepository, ILogger<CreateGameUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<CreateGameResponse> ExecuteAsync(CreateGameRequest request)
        {
            _logger.LogInformation("Executing {UseCase}. Title={Title}, ReleaseDate={ReleaseDate}", nameof(CreateGameUseCase), request.Title, request.ReleaseDate);

            if (!Enum.TryParse<AgeRating>(request.AgeRating, true, out var ageRating))
            {
                _logger.LogWarning("Invalid age rating. AgeRating={AgeRating}, Title={Title}", request.AgeRating, request.Title);
                throw new ApplicationException($"Invalid age rating '{request.AgeRating}'");
            }

            var gameModes = new List<GameModes>();
            foreach (var requestGameMode in request.GameModes)
            {
                if (!Enum.TryParse<GameModes>(requestGameMode, true, out var gameMode))
                {
                    _logger.LogWarning("Invalid game mode. GameMode={GameMode}, Title={Title}", requestGameMode, request.Title);
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

            try
            {
                var game = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Game.CreateGame(
                    request.Title,
                    request.Description,
                    request.Story,
                    request.Franchise,
                    request.ReleaseDate,
                    ageRating,
                    gameModes,
                    publisher,
                    developers);

                await _gameRepository.AddAsync(game);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Game created successfully. GameId={GameId}, Title={Title}", game.Id, game.Title);

                var response = new CreateGameResponse()
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
                    Publisher = publisher.Name,
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing {UseCase}. Title={Title}", nameof(CreateGameUseCase), request.Title);
                throw;
            }
        }
    }
}
