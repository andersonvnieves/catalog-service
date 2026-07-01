using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Story).IsRequired().HasMaxLength(5000);
            builder.Property(x => x.Franchise).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ReleaseDate).IsRequired();
            builder.Property(x => x.AgeRating).HasConversion<String>().IsRequired();

            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);

            builder.Property(x => x.GameModes)
                .IsRequired()
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<List<GameModes>>(v, jsonOptions) ?? new List<GameModes>())
                .Metadata.SetValueComparer(new ValueComparer<List<GameModes>>(
                    (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
                    v => (v ?? new()).Aggregate(0, (acc, item) => HashCode.Combine(acc, item.GetHashCode())),
                    v => (v ?? new()).ToList()
                ));

            builder.OwnsMany(x => x.Developers, x =>
            {
                x.WithOwner().HasForeignKey("GameId");
                x.Property<int>("Id");
                x.HasKey("Id");
                x.Property(m => m.Name).HasMaxLength(150).IsRequired();
            });            

            builder.OwnsOne(x => x.Publisher, x =>
            {
                x.Property(m => m.Name).HasMaxLength(150).IsRequired();
            });

            builder.Property(x => x.Price.PriceValue).IsRequired();
        }
    }
}
