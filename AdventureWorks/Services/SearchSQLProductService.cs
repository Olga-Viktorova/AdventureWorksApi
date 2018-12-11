using System;
using AdventureWorksApi.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;

namespace AdventureWorksApi.Services
{
  public class SearchSQLProductService
  {
    private readonly IConfiguration _configuration;

    public SearchSQLProductService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public DocumentSearchResult<Product> RunQueries(ISearchIndexClient indexClient, string searchString)
    {
      SearchParameters parameters;
      DocumentSearchResult<Product> results;

      parameters =
        new SearchParameters()
        {
          Select = new[] { "Name", "ReorderPoint" }
        };

      results = indexClient.Documents.Search<Product>(searchString, parameters);

      #region Other Search options
      //WriteDocuments(results);

      //Console.Write("Apply a filter to the index to find product name where reorder point less then 600, ");
      //Console.WriteLine("and return the hotelId and description:\n");

      //parameters =
      //  new SearchParameters()
      //  {
      //    Filter = "ReorderPoint lt 600",
      //    Select = new[] { "Name", "ReorderPoint" }
      //  };

      //results = indexClient.Documents.Search<Product>("*", parameters);

      //WriteDocuments(results);

      //Console.Write("Search the entire index, order by Name ");
      //Console.Write("in descending order, take the top two results, and show only Name and ");
      //Console.WriteLine("Reorder point:\n");

      //parameters =
      //  new SearchParameters()
      //  {
      //    OrderBy = new[] { "ReorderPoint desc" },
      //    Select = new[] { "Name", "ReorderPoint" },
      //    Top = 2
      //  };

      //results = indexClient.Documents.Search<Product>("*", parameters);

      //WriteDocuments(results);

      //Console.Read();
      #endregion

      return results;
    }

    public SearchIndexClient CreateSearchIndexClient()
    {
      string searchServiceName = _configuration["SearchServiceName"];
      string queryApiKey = _configuration["SearchServiceQueryApiKey"];

      SearchIndexClient indexClient = new SearchIndexClient(searchServiceName, "azuresql-index", new SearchCredentials(queryApiKey));
      return indexClient;
    }
  }
}
