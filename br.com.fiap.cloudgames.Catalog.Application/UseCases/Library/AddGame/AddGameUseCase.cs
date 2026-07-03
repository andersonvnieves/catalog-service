using System;
using System.Collections.Generic;
using System.Text;
using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Library.AddGame
{
    public class AddGameUseCase
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IUnitOfWork _unitOfWork;
        
        public AddGameUseCase(IGameRepository gameRepository, ILibraryRepository libraryRepository, IUnitOfWork unitOfWork)
        {
            _gameRepository = gameRepository;
            _libraryRepository = libraryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AddGameResponse> ExecuteAsync(AddGameRequest request)
        {
            if(request.UserId == Guid.Empty)
                throw new ApplicationException("User not found");
            if(request.OrderId == Guid.Empty)
                throw new ApplicationException("Order not found");
            if(request.GameIds.Count == 0)
                throw new ApplicationException("No games selected");
            
            var games = await _gameRepository.GetByIdsAsync(request.GameIds);
            if(games.Count() != request.GameIds.Count)
                throw new ApplicationException("One or more games not found");
            
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var library = await _libraryRepository.GetByIdAsync(request.UserId);
                if (library == null)
                {
                    //Create User Library
                    library = br.com.fiap.cloudgames.Catalog.Domain.Aggregates.Library.Create(request.UserId);
                    await _libraryRepository.AddAsync(library);
                }

                //Add Game to User Library
                foreach (var game in games)
                {
                    library.AddGame(new OwnedGame(game.Id, request.OrderId, DateTime.Now));
                }

                _libraryRepository.UpdateAsync(library);
                _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackAsync();
                throw;
            }
            
            return new AddGameResponse()
            {
                
            };
        }
    }
}
