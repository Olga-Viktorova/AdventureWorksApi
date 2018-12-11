using System.Collections.Generic;
using AdventureWorksApi.Models;
using AdventureWorksApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search.Models;

namespace AdventureWorksApi.Controllers
{
  [Route("[controller]")]
  public class SearchController : Controller
  {
    private readonly SearchSQLProductService _sqlService;

    public SearchController(SearchSQLProductService sqlService)
    {
      _sqlService = sqlService;
    }
    
    // GET: Search
    public ActionResult Index(string searchString)
    {
      #region 
      //IQueryable<Product> products = null;
      //products = string.IsNullOrEmpty(searchString) == false 
      //  ? _context.Product.Where(p => p.Name.Contains(searchString)) : _context.Product;


      //var products = from p in _context.Product
      //               select p;

      //if (!String.IsNullOrEmpty(searchString))
      //{
      //  products = products.Where(s => s.Name.Contains(searchString));
      //}
      #endregion

      var products = new List<Product>();

      var sqlClient = _sqlService.CreateSearchIndexClient();
      var results = _sqlService.RunQueries(sqlClient, searchString);
     
      foreach (SearchResult<Product> result in results.Results)
      {
        products.Add(result.Document);
      }

      return View(products);
    }
  }
}