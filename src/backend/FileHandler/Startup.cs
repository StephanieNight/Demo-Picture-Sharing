using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using picture_sharing.Services;
using Services;
using System;
[assembly: FunctionsStartup(typeof(FileHandler.Startup))]
namespace FileHandler
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            var keyvault = Environment.GetEnvironmentVariable("KeyVault");

            // == Services == 
            //var keyVaultUri = $"https://{keyvault}.vault.azure.net";
            var keyVaultUri = $"https://kv-wedding.vault.azure.net/";
            KeyVaultService keyVaultService = new KeyVaultService(keyVaultUri);
            StorageService storage = new StorageService(keyVaultService);

            services.AddSingleton(keyVaultService);
            services.AddSingleton(storage);

            // == Builds provider ==
            services.BuildServiceProvider();
        }
    }
}