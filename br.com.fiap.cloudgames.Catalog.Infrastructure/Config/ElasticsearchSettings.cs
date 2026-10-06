namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Config;

public class ElasticsearchSettings
{
    public string Url { get; set; }
    public string DefaultIndex { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}