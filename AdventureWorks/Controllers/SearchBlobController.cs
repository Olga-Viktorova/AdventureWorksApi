using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorksApi.Models;
using AdventureWorksApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search.Models;
using SearchIndex;

namespace AdventureWorksApi.Controllers
{
  public class SearchBlobController : Controller
  {
    private readonly SearchBlobService _blobService;

    public SearchBlobController(SearchBlobService blobService)
    {
      _blobService = blobService;
    }

    public IActionResult Index(string searchString)
    {
      var products = new List<Blob>();

      var blobClient = _blobService.CreateSearchIndexClient();
      var results = _blobService.RunQueries(blobClient, searchString);

      foreach (SearchResult<Blob> result in results.Results)
      {
        products.Add(result.Document);
      }

      return View(products);
    }
  }
}