using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly ILogger<GameRepository> _logger;
        private readonly AppDbContext _context;
        private readonly DbSet<Game> _games;

        public GameRepository(AppDbContext context)
        {
            _context = context;
            _games = context.Games;
        }

        public async Task AddAsync(Game game)
        {
            await _games.AddAsync(game);
        }

        public async Task<Game?> GetByIdAsync(Guid id)
        {
            return await _games.FirstOrDefaultAsync(g => g.Id == id);
        }

        public void Update(Game game)
        {
            _games.Update(game);
        }
    }
}
