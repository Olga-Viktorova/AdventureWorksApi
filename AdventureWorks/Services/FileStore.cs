using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AdventureWorksApi.Services
{
  public class FileStore
  {
    private CloudBlobClient blobClient;
    private CloudQueueClient queueClient;
    //private string baseUriBlob = "https://storageaccountforblobs.blob.core.windows.net/";
    //private string baseUriQueue = "https://storageaccountforblobs.queue.core.windows.net/";
    private string baseUriBlob = "https://storageaccountaw.blob.core.windows.net/";
    private string baseUriQueue = "https://storageaccountaw.queue.core.windows.net/";

    public FileStore()
    {
      //epam subscription
      //var credentials = new StorageCredentials("storageaccountforblobs",
      //  "14un74pc2jpX10ld2zvb8ZWqL2H7DSQhr2oHtXomnWIby7PLf33XhtgzyatIgg1FkqlxVlOsoEApEJU0hK70kw==");

      //personal
      var credentials = new StorageCredentials("storageaccountaw",
        "43VFe0AQezMlebphIDJ98f6YMe0uC0CpRjELvIy/424GAYCQMJ2KJEfKFU6c1nZVAjRpWyfyBr8tY5RqA94klg==");
      
      blobClient = new CloudBlobClient(new Uri(baseUriBlob), credentials);
      queueClient = new CloudQueueClient(new Uri(baseUriQueue), credentials);
    }

    public async Task<string> SaveFile(Stream stream)
    {
      var fileId = Guid.NewGuid().ToString();
      var container = blobClient.GetContainerReference("files");
      var blob = container.GetBlockBlobReference(fileId);
      await blob.UploadFromStreamAsync(stream);

      return fileId;
    }

    public async Task AddMessageToQueue(string fileIdBlob, string fileId)
    {
      CloudQueue messageQueue = queueClient.GetQueueReference("awapiqueue");

      var textMmessage = "File " + fileId + " was added successfully ";
      textMmessage +=  "BlobId " + fileIdBlob;
     // textMmessage += "Metadata: " + metadata;
      CloudQueueMessage message = new CloudQueueMessage(textMmessage);

      await messageQueue.AddMessageAsync(message);
    }

    public string UriFor(string fileId)
    {
      return $"{baseUriBlob}files/{fileId}";
    }
  }
}
