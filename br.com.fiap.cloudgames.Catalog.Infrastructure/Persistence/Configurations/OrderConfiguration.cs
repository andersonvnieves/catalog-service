using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.OwnsMany(x => x.Items, x =>
            {
                x.WithOwner().HasForeignKey("OrderId");
                x.HasKey("OrderId", "GameId");
                x.Property(x => x.GameId);
                x.OwnsOne(i => i.Price, p =>
                {
                    p.Property(x => x.PriceValue)
                        .HasColumnName("Price")
                        .IsRequired();
                });
            });
            builder.Navigation(x => x.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Property(x => x.OrderStatus).HasConversion<String>().IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}
