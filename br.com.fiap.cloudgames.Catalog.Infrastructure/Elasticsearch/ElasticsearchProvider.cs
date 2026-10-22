using br.com.fiap.cloudgames.Catalog.Infrastructure.Config;
using Microsoft.Extensions.Options;
using OpenSearch.Client;
using OpenSearch.Net;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Elasticsearch;

public class ElasticsearchProvider
{
    public OpenSearchClient Client { get; }
    public string DefaultIndex { get; }

    public ElasticsearchProvider(IOptions<ElasticsearchSettings> settings)
    {
        DefaultIndex = settings.Value.DefaultIndex;
        
        var connectionSettings = new ConnectionSettings(new Uri(settings.Value.Url))
            .DefaultIndex(DefaultIndex);
       
        if (!string.IsNullOrEmpty(settings.Value.Username) && 
            !string.IsNullOrEmpty(settings.Value.Password))
        {
            connectionSettings.BasicAuthentication(settings.Value.Username, settings.Value.Password);
        }
        
        Client = new OpenSearchClient(connectionSettings);
    }
}
