using System.Threading.Tasks;
using core.Interfaces;
using core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace core.Factories
{
    public class ProductServiceFactory : IProductServiceFactory
    {
        private IConfiguration Configuration { get; }
        public ProductServiceFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ProductService GetProductService()
        {
            var azureConnString = Configuration.GetConnectionString("azureConnString");

            var storageAccount = CloudStorageAccount.Parse(azureConnString);

            var tableClient = storageAccount.CreateCloudTableClient();

            var productTable = tableClient.GetTableReference("Products");

            Task.Run(productTable.CreateIfNotExistsAsync).GetAwaiter().GetResult();

            return new ProductService(productTable);
        }
    }

}
