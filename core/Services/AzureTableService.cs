using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace core.Services
{
    public abstract class AzureTableService<T>
        where T : TableEntity, new()
    {
        protected CloudTable Table { get; set; }

        protected AzureTableService(CloudTable table)
        {
            Table = table;
        }

        public async Task Add(T entity)
        {
            var insertOperation = TableOperation.Insert(entity);

            await Table.ExecuteAsync(insertOperation);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var query = new TableQuery<T>();

            var entities = new List<T>();

            TableContinuationToken token = null;
            do
            {
                var resultSegment = await Table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                entities.AddRange(resultSegment.Results);

            } while (token != null);

            return entities;
        }

        public async Task<T> GetSingle(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
           
            var retrievedResult = await Table.ExecuteAsync(retrieveOperation);

            return (T)retrievedResult.Result;
        }

        public async Task DeleteSingle(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            var retrievedResult = await Table.ExecuteAsync(retrieveOperation);

            var deleteEntity = (T)retrievedResult.Result;

            if (deleteEntity != null)
            {
                var deleteOperation = TableOperation.Delete(deleteEntity);

                await Table.ExecuteAsync(deleteOperation);
            }
        }
    }
}
