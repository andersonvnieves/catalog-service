namespace br.com.fiap.cloudgames.Catalog.Application.Consumers;

public interface IMessageConsumer : IAsyncDisposable
{
    Task ConsumeAsync();
}