using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace SearchIndex
{
  [SerializePropertyNamesAsCamelCase]
  public class Blob
  {
    [IsSearchable, IsFilterable, IsSortable, IsFacetable]
    public string Content { get; set; }
  }
}
