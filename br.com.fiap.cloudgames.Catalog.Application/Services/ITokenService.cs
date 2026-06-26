using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;

namespace br.com.fiap.cloudgames.Catalog.Application.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user); 
}