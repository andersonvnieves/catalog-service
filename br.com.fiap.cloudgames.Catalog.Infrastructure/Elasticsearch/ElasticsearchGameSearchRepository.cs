using br.com.fiap.cloudgames.Catalog.Application.DTOs;
using br.com.fiap.cloudgames.Catalog.Application.Repositories;
using Elastic.Clients.Elasticsearch;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Elasticsearch;

public class ElasticsearchGameSearchRepository : IGameSearchRepository
{
    private readonly ElasticsearchProvider _elasticsearchProvider;
    
    public ElasticsearchGameSearchRepository(ElasticsearchProvider elasticsearchProvider)
    {
        _elasticsearchProvider = elasticsearchProvider;
    }
    
    public async Task<IEnumerable<GameIndexDocument>> SearchAsync(string query)
    {
        var searchResponse = await _elasticsearchProvider.Client.SearchAsync<GameIndexDocument>(s => s
            .Query(q => q
                .Bool(b => b
                    .Must(m => m
                        .MultiMatch(mm => mm
                            .Query(query)
                            .Fields(new[] { "title^3", "description", "story" })
                            .Fuzziness(new Fuzziness("AUTO"))
                        )
                    )
                )
            )
        );

        if (!searchResponse.IsValidResponse)
        {
            return Enumerable.Empty<GameIndexDocument>();
        }

        return searchResponse.Documents.ToList();
    }
}