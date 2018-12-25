using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AdventureWorksApi.Services
{
  public class ConnectionStringService
  {
    public static async Task<ConnectionStringService> GetInstanceAsync()
    {
      var instance = new ConnectionStringService();
      await instance.OnGetAsync();

      return instance;
    }

    public string Message { get; set; }

    private async Task OnGetAsync()
    {
      Message = "Your application description page.";
      int retries = 0;
      bool retry = false;
      try
      {
        /* The below 4 lines of code shows you how to use AppAuthentication library to fetch secrets from your Key Vault*/
        AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
        KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        var secret = await keyVaultClient.GetSecretAsync("https://keyvaultapi1.vault.azure.net/secrets/ConnectionString/")
                .ConfigureAwait(false);
        Message = secret.Value;


        //var context = new AuthenticationContext("https://login.windows.net/" + "f602ffea-4a91-452e-83f0-a8e3a75dc29e");
        //ClientCredential clientCredential = new ClientCredential(appId, secretKey);
        //var tokenResponse = await context.AcquireTokenAsync("https://vault.azure.net", clientCredential);
        //var accessToken = tokenResponse.AccessToken;
        //return accessToken;

        /* The below do while logic is to handle throttling errors thrown by Azure Key Vault. It shows how to do exponential backoff which is the recommended client side throttling*/
        do
        {
          long waitTime = Math.Min(getWaitTime(retries), 2000000);
          secret = await keyVaultClient.GetSecretAsync("https://keyvaultapi1.vault.azure.net/secrets/ConnectionString/")
              .ConfigureAwait(false);
          retry = false;
        }
        while (retry && (retries++ < 10));
      }

      /// <exception cref="KeyVaultErrorException">
      /// Thrown when the operation returned an invalid status code
      /// </exception>
      catch (KeyVaultErrorException keyVaultException)
      {
        Message = keyVaultException.Message;
        if ((int)keyVaultException.Response.StatusCode == 429)
          retry = true;
      }
    }

    // This method implements exponential backoff incase of 429 errors from Azure Key Vault
    private static long getWaitTime(int retryCount)
    {
      long waitTime = ((long)Math.Pow(2, retryCount) * 100L);
      return waitTime;
    }

    // This method fetches a token from Azure Active Directory which can then be provided to Azure Key Vault to authenticate
    //public async Task<string> GetAccessTokenAsync()
    //{
    //  var azureServiceTokenProvider = new AzureServiceTokenProvider();
    //  string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://vault.azure.net");
    //  return accessToken;
    //}
  }
}
