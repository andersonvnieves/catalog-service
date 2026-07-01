namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Config;

public class JwtTokenSettings
{
    public required String Issuer { get; set; }
    public required String Audience { get; set; }
    public required String Key { get; set; }
    public int TokenTtlInMinutes { get; set; }
}