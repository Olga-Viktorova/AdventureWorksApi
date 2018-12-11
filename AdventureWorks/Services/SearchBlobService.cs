using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;
using SearchIndex;

namespace AdventureWorksApi.Services
{
  public class SearchBlobService
  {
    private readonly IConfiguration _configuration;

    public SearchBlobService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public DocumentSearchResult<Blob> RunQueries(ISearchIndexClient indexClient, string searchString)
    {
      SearchParameters parameters;
      DocumentSearchResult<Blob> results;

      parameters =
        new SearchParameters()
        {
          Select = new[] { "content" }
        };

      results = indexClient.Documents.Search<Blob>(searchString, parameters);

      return results;
    }

    public SearchIndexClient CreateSearchIndexClient()
    {
      string searchServiceName = _configuration["SearchServiceName"];
      string queryApiKey = _configuration["SearchServiceQueryApiKey"];

      SearchIndexClient indexClient = new SearchIndexClient(searchServiceName, "azureblob-index", new SearchCredentials(queryApiKey));
      return indexClient;
    }
  }
}
