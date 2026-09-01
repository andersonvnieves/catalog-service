using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.MongoDB.Configurations
{
    public static class LibraryMongoMap
    {
        public static void Configure()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Library)))
            {
                BsonClassMap.RegisterClassMap<Library>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(c => c.UserId).SetIdGenerator(CombGuidGenerator.Instance);
                    cm.MapField("_ownedGames").SetElementName("ownedGames");
                    cm.UnmapProperty(c => c.OwnedGames);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(OwnedGame)))
            {
                BsonClassMap.RegisterClassMap<OwnedGame>(cm =>
                {
                    cm.AutoMap();
                    cm.MapCreator(og => new OwnedGame(og.GameId, og.OrderId, og.PurchaseDate));
                });
            }
        }
    }
}
