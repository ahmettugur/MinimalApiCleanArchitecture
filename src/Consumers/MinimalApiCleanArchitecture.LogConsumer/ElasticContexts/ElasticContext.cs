using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Microsoft.Extensions.Options;
using MinimalApiCleanArchitecture.LogConsumer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MinimalApiCleanArchitecture.LogConsumer.ElasticContexts;

public class ElasticContext : IElasticContext
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly int _pingTimeMilliSeconds;
    private const string LogAlias = "_log";
    private readonly JsonSerializerSettings _jsonSettings;

    public ElasticContext(IOptions<ElasticSearchConfigModel> configuration)
    {
        _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new LowercaseContractResolver()
        };

        _pingTimeMilliSeconds = configuration.Value.PingTimeMilliSeconds;

        var settings = new ElasticsearchClientSettings(new Uri(configuration.Value.ConnectionString))
            .RequestTimeout(TimeSpan.FromMilliseconds(_pingTimeMilliSeconds))
            .DisableDirectStreaming();

        _elasticClient = new ElasticsearchClient(settings);


    }

    public IndexResponseModel IndexCustom<T>(string indexName, T document) where T : class?
    {
        indexName = GetIndexName(indexName);
        var isExistsTask = _elasticClient.Indices.ExistsAsync(indexName);
        isExistsTask.Wait();
        var isExists = isExistsTask.Result;
        if (!isExists.Exists)
        {
            var createIndexResponseModel = CreateIndexCustom<T>(indexName).Result;
            if (!createIndexResponseModel.IsValid)
                return createIndexResponseModel;
        }
        var responseTask = _elasticClient.IndexAsync(document, idx => idx.Index(indexName));
        responseTask.Wait();
        var response = responseTask.Result;
        return new IndexResponseModel { IsValid = response.IsValidResponse };
    }
    public async Task<IndexResponseModel> IndexCustomAsync<T>(string indexName, T document, CancellationToken ct = default) where T : class?
    {
        indexName = GetIndexName(indexName);
        var isExists = await _elasticClient.Indices.ExistsAsync(indexName, ct);
        if (!isExists.Exists)
        {
            var createIndexResponseModel = await CreateIndexCustom<T>(indexName, ct);
            if (!createIndexResponseModel.IsValid)
                return createIndexResponseModel;
        }
        var response = await _elasticClient.IndexAsync(document, idx => idx.Index(indexName), ct);
        return new IndexResponseModel { IsValid = response.IsValidResponse };
    }

    private static string GetIndexName(string indexName)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var index = $"{indexName.ToLowerInvariant()}_{DateTime.Now:yyyy.MM.dd}";

        if (!string.IsNullOrEmpty(env))
        {
            index = $"{env.Trim().ToLower()}_{index}";
        }
        return index.Replace(".", "_").Replace("-", "_");
    }

    private async Task<IndexResponseModel> CreateIndexCustom<T>(string indexName, CancellationToken ct = default) where T : class?
    {
        var indexSettings = new IndexSettings
        {
            NumberOfReplicas = 1,
            NumberOfShards = 1,

        };

        var response = await _elasticClient.Indices.CreateAsync(indexName, ct);

        return new IndexResponseModel
        {
            IsValid = response.IsValidResponse
        };
    }
}

public class LowercaseContractResolver : DefaultContractResolver
{
    protected override string ResolvePropertyName(string propertyName)
    {
        return propertyName.ToLower();
    }
}