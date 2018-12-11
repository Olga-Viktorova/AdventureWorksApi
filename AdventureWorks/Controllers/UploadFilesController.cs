using System.Threading.Tasks;
using AdventureWorksApi.Models;
using AdventureWorksApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorksApi.Controllers
{
  [Route("[controller]")]
  public class UploadFilesController : Controller
  {
    private readonly FileStore fileStore;

    public UploadFilesController(FileStore fileStore)
    {
      this.fileStore = fileStore;
    }

    public IActionResult Index()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
      if (file != null)
      {
        using (var stream = file.OpenReadStream())
        {
          var fileId = await fileStore.SaveFile(stream);

          //var metadata = new byte[fileStream.Length];
          //await fileStream.ReadAsync(metadata, 0, (int)fileStream.Length);
          //metadata.ToString();

          await fileStore.AddMessageToQueue(fileId, file.FileName);

          return RedirectToAction("Show", new { fileId });
        }
      }
      return View();
    }

    [HttpGet("{fileId}")]
    public ActionResult Show(string fileId)
    {
      var model = new ShowModel { Uri = fileStore.UriFor(fileId) };

      return View(model);
    }

  }
}