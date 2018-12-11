using System.Configuration;
using AdventureWorksApi.Models;
using AdventureWorksApi.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Serilog;
using Serilog.Settings.Configuration;
using Swashbuckle.AspNetCore.Swagger;

namespace AdventureWorksApi
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

      //CloudStorageAccount storageAccount = new CloudStorageAccount(
      //  new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
      //    "awapistorageaccount", "rHIgf9+q80ScT8LSybX/hYyYq6xwdJZ44KFQ/A3dJU3s4kl5MLyUKaTb3xGjEgUaFkYAR/9vUQqDQiZT/0e0Ew=="), true);

      //CloudStorageAccount storageAccount = new CloudStorageAccount(
      //  new StorageCredentials(this.settings.StorageAccount, this.settings.StorageKey), false);

      //var storage = CloudStorageAccount.FromConfigurationSetting("MyStorage");

      Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

      //Log.Logger = new LoggerConfiguration()
      //  .WriteTo.AzureTableStorage(storage)
      //  .CreateLogger();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
      services.AddScoped<FileStore, FileStore>();
      services.AddScoped<SearchSQLProductService, SearchSQLProductService>();
      services.AddScoped<SearchBlobService, SearchBlobService>();

      //var connection = @"Server=mysqlvmlabel.westeurope.cloudapp.azure.com,1433;Database=AdventureWorks2012;user id=volha_viktarava;password=123456qwerty!@;Trusted_Connection=True;ConnectRetryCount=0";
      //var connection = @"data source=mysqlvmlabel.westeurope.cloudapp.azure.com,1433;initial catalog=AdventureWorks2012;integrated security=false;user id=volha_viktarava;password=123456qwerty!@;MultipleActiveResultSets=True;";
      //var connection =
      //  @"data source=localhost;initial catalog = AdventureWorks2012; persist security info = True;Integrated Security = SSPI;"; 

      // personal subscription
      var connection =
        @"Server= adventureworks2server.database.windows.net,1433;Initial Catalog=AdventureWorks2;Persist Security Info=False;User ID=volha_viktarava;Password=123456qwerty!@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

      //// epam subscription
      //var connection =
      //  @"Server=tcp:adventureworksserver12.database.windows.net,1433;Initial Catalog=AdventureWorks;Persist Security Info=False;User ID=volha_viktarava;Password=123456qwerty!@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

      services.AddDbContext<AdventureWorksDbContext>(options => options.UseSqlServer(connection));

      services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info {Title = "My API", Version = "v1"}));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();

      app.UseSwagger();
      app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });

      loggerFactory.AddSerilog();
    }

    public static IWebHost BuildWebHost(string[] args) =>
      WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().UseSerilog().Build();
  }
}
