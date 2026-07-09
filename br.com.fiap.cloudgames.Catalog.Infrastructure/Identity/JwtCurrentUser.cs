using br.com.fiap.cloudgames.Catalog.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Identity
{
    public class JwtCurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId =>
            Guid.Parse(_httpContextAccessor.HttpContext!.User
                .FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

        public string Name =>
            _httpContextAccessor.HttpContext!.User
                .FindFirst(JwtRegisteredClaimNames.Name)!.Value;

        public string Email =>
            _httpContextAccessor.HttpContext!.User
                .FindFirst(JwtRegisteredClaimNames.Email)!.Value;
    }
}
