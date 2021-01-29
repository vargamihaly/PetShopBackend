using System.Threading.Tasks;
using core.Interfaces;
using core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace core.Factories
{
    public class ReviewServiceFactory : IReviewServiceFactory
    {
        private IConfiguration Configuration { get; }
        public ReviewServiceFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ReviewService GetReviewService()
        {
            var azureConnString = Configuration.GetConnectionString("azureConnString");

            var storageAccount = CloudStorageAccount.Parse(azureConnString);

            var tableClient = storageAccount.CreateCloudTableClient();

            var reviewTable = tableClient.GetTableReference("Reviews");

            Task.Run(reviewTable.CreateIfNotExistsAsync).GetAwaiter().GetResult();

            return new ReviewService(reviewTable);
        }
    }
}
