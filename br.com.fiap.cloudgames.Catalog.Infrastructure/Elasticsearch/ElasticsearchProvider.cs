using br.com.fiap.cloudgames.Catalog.Infrastructure.Config;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Options;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Elasticsearch;

public class ElasticsearchProvider
{
    public ElasticsearchClient Client { get; }

    public ElasticsearchProvider(IOptions<ElasticsearchSettings> settings)
    {
        var clientSettings = new ElasticsearchClientSettings(new Uri(settings.Value.Url))
            .DefaultIndex(settings.Value.DefaultIndex);
       
        if (!string.IsNullOrEmpty(settings.Value.Username) && 
            !string.IsNullOrEmpty(settings.Value.Password))
        {
            clientSettings.Authentication(new BasicAuthentication(settings.Value.Username, settings.Value.Password));
        }
        
        Client = new ElasticsearchClient(clientSettings);
    }
}