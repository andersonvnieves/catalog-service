using br.com.fiap.cloudgames.Catalog.Application.Abstractions;
using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Relational.Repositories.Cached
{
    public class CachedGameRepository : IGameRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IGameRepository _gameRepository;
        

        public CachedGameRepository(ICacheProvider cacheProvider, IGameRepository gameRepository) 
        {
            _cacheProvider = cacheProvider;
            _gameRepository = gameRepository;
        }

        public async Task AddAsync(Game game)
        {
            await _cacheProvider.DeleteDataAsync(GameKey(game.Id));
            await _gameRepository.AddAsync(game);
            await _cacheProvider.SetDataAsync(GameKey(game.Id), game);
        }

        public async Task<Game?> GetByIdAsync(Guid id)
        {
            var cachedGame = await _cacheProvider.GetDataAsync<Game>(GameKey(id));
            if (cachedGame != null)
                return cachedGame;

            var dbGame = await _gameRepository.GetByIdAsync(id);
            if (dbGame == null) 
                return null;

            await _cacheProvider.SetDataAsync(GameKey(id), dbGame); 
            
            return dbGame;
        }

        public async Task<IEnumerable<Game>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            if (!ids.Any()) 
                return Enumerable.Empty<Game>();

            var keyMap = ids.ToDictionary(id => GameKey(id), id => id);
            var cachedResults = await _cacheProvider.GetBatchDataAsync<Game>(keyMap.Keys);

            var games = new List<Game>();
            var missingIds = new List<Guid>();
            foreach (var kvp in cachedResults)
            {
                var gameId = keyMap[kvp.Key];
                if (kvp.Value != null)
                {
                    games.Add(kvp.Value);
                }
                else
                {
                    missingIds.Add(gameId);
                }
            }

            var dbGames = new List<Game>();
            if (missingIds.Any())
            {
                dbGames = (await _gameRepository.GetByIdsAsync(missingIds)).ToList();
                if (!dbGames.Any())
                    return games;

                var gamesToCache = dbGames.ToDictionary(game => GameKey(game.Id), game => game);
                await _cacheProvider.SetBatchDataAsync(gamesToCache);
            }

            return games.Concat(dbGames);
        }

        public async Task UpdateAsync(Game game)
        {
            await _cacheProvider.SetDataAsync(GameKey(game.Id), ICacheProvider.UPDATING_LOCK, TimeSpan.FromSeconds(30));
            await _gameRepository.UpdateAsync(game);
        }

        private string GameKey(Guid id) => $"game:{id.ToString()}";
    }
}
