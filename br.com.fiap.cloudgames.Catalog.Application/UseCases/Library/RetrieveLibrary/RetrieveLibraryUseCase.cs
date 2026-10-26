using br.com.fiap.cloudgames.Catalog.Application.Abstractions;
using br.com.fiap.cloudgames.Catalog.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using LibraryAggregate = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Library.RetrieveLibrary
{
    public class RetrieveLibraryUseCase
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ICurrentUser _currentUser;

        public RetrieveLibraryUseCase(ILibraryRepository libraryRepository, IGameRepository gameRepository, ICurrentUser currentUser)
        {
            _libraryRepository = libraryRepository;
            _gameRepository = gameRepository;
            _currentUser = currentUser;
        }

        public async Task<RetrieveLibraryResponse> ExecuteAsync()
        {
            var library = await _libraryRepository.GetByUserIdAsync(_currentUser.UserId);
            if (library == null)
            {
                library = LibraryAggregate.Create(_currentUser.UserId);
                await _libraryRepository.AddAsync(library);
            }
            
            var games = await _gameRepository.GetByIdsAsync(library.OwnedGames.Select(x => x.GameId));
            
            var gamesDictionary = new Dictionary<string, string>();
            foreach (var game in games)
            {
                gamesDictionary.Add(game.Id.ToString(), game.Title);
            }
            
            return new RetrieveLibraryResponse()
            {
                OwnedGames = gamesDictionary
            };
        }
    }
}
