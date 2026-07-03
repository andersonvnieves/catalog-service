using System;
using System.Collections.Generic;
using System.Text;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Library.RetrieveLibrary
{
    public class RetrieveLibraryUseCase
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IGameRepository _gameRepository;
        
        public RetrieveLibraryUseCase(ILibraryRepository libraryRepository, IGameRepository gameRepository)
        {
            _libraryRepository = libraryRepository;
            _gameRepository = gameRepository;
        }

        public async Task<RetrieveLibraryResponse> ExecuteAsync(RetrieveLibraryRequest request)
        {
            var library = await _libraryRepository.GetByIdAsync(request.UserId);
            if(library == null)
                throw new ApplicationException("No Library found");
            
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
