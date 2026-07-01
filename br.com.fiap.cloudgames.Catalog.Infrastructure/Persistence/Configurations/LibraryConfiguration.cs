using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Configurations
{
    public class LibraryConfiguration : IEntityTypeConfiguration<Library>
    {
        public void Configure(EntityTypeBuilder<Library> builder)
        {
            builder.HasKey(x => x.UserId);

            builder.OwnsMany(x => x.OwnedGames, game =>
            {
                game.WithOwner().HasForeignKey("UserId");
                game.Property<Guid>("UserId");
                game.HasKey("UserId", "GameId");
                game.Property(x => x.GameId);
                game.Property(x => x.OrderId);
                game.Property(x => x.PurchaseDate);
            });
        }
    }
}
