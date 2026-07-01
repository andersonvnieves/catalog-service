using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Domain.Aggregates
{
    public class Library
    {
        public Library() { } //ORM

        #region FactoryMethod
        public static Library Create(Guid userId)
        {
            Validate(userId);

            return new Library() { 
                UserId = userId,
            }; 
        }
        #endregion

        #region Properties
        public Guid UserId { get; set; }
        private List<OwnedGame> _ownedGames = new();
        public IReadOnlyCollection<OwnedGame> OwnedGames => _ownedGames;
        #endregion

        private static void Validate(Guid userId)
        {
            var errors = new List<string>();

            if (userId == Guid.Empty)
                errors.Add("UserId is required.");

            
            if (errors.Any())
                throw new DomainException(errors);
        }

        public void AddGame(OwnedGame game)
        {
            if(_ownedGames.Any(x => x.GameId == game.GameId))
                throw new DomainException("User already owns this game.");
            
            _ownedGames.Add(game);
        }

        public bool OwnsGame(Guid gameId)
        {
            return _ownedGames.Any(g => g.GameId == gameId);
        }

        public bool OwnsGames(IEnumerable<Guid> gameIds)
        {
            return gameIds.Any(id => OwnsGame(id));
        }
    }
}
