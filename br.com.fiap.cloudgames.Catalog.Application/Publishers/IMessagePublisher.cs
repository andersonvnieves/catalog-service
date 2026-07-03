
namespace br.com.fiap.cloudgames.Catalog.Application.Publishers
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message);
    }
}
