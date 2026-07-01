using br.com.fiap.cloudgames.Catalog.Domain.Entities;
using br.com.fiap.cloudgames.Catalog.Domain.Enums;
using br.com.fiap.cloudgames.Catalog.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Catalog.Domain.Tests.TestData;

public static class DomainTestData
{
    public static Publisher ValidPublisher() => new("Publisher Inc");
    public static List<Developer> ValidDevelopers() => [new Developer("Dev Studio")];
    public static List<GameModes> ValidGameModes() => [GameModes.SinglePlayer];
    public static Price ValidPrice() => new Price(10);
}

