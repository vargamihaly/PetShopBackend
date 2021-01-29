using System;
using Microsoft.WindowsAzure.Storage.Table;
using core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using core.DTO;
using System.Linq;


namespace core.Services
{
    public class ProductService : AzureTableService<Product>
    {
        public CloudTable ProuctTable { get; }

        public ProductService(CloudTable productTable) : base(productTable)
        {
            ProuctTable = productTable;
        }

        public async Task<ServiceResponse<IEnumerable<ProductDto>>> GetAllProduct()
        {
            var response = new ServiceResponse<IEnumerable<ProductDto>>();

            try
            {
                var products = await GetAll();
                if (products == null)
                {
                    response.Success = false;
                    response.Message = "There are no products to show.";

                    return response;
                }

                response.Success = true;
                response.Data = products.OrderBy(x => x.PartitionKey).ThenBy(x => x.RowKey).Select(x => new ProductDto
                {
                    ProductCategory = x.PartitionKey,
                    ProductName = x.RowKey,
                    Description = x.Description,
                    UnitPrice = x.UnitPrice,
                    UnitsInStock = x.UnitsInStock
                });

                return response;

            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = $"There was an error getting the products.";
                return response;
            }
        }

        public async Task<ServiceResponse<IEnumerable<ProductDto>>> GetAllProductWithinCategory(string category)
        {
            var response = new ServiceResponse<IEnumerable<ProductDto>>();

            try
            {
                var query = new TableQuery<Product> { FilterString = $"PartitionKey eq '{category}'" };

                List<Product> products = new List<Product>();

                TableContinuationToken token = null;

                var resultSegment = await Table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                products.AddRange(resultSegment.Results);

                if (!products.Any())
                {
                    response.Success = false;
                    response.Message = $"There are no products in '{category}' category.";

                    return response;
                }

                response.Success = true;
                response.Data = products.OrderBy(x => x.PartitionKey).ThenBy(x => x.RowKey).Select(x => new ProductDto
                {
                    ProductCategory = x.PartitionKey,
                    ProductName = x.RowKey,
                    Description = x.Description,
                    UnitPrice = x.UnitPrice,
                    UnitsInStock = x.UnitsInStock
                });

                return response;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = $"There was an error getting the products.";
                return response;
            }
        }

        public async Task<ServiceResponse<ProductDto>> GetProduct(string productCategory, string productName)
        {
            var response = new ServiceResponse<ProductDto>();

            try
            {
                var product = await GetSingle(productCategory, productName);

                response.Success = true;
                response.Data = new ProductDto
                {
                    ProductCategory = product.PartitionKey,
                    ProductName = product.RowKey
                };

                return response;

            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = $"There was an error getting the products.";
                return response;
            }
        }


        public async Task<ServiceResponse<string>> AddProduct(ProductDto product)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var prevProduct = await GetSingle(product.ProductCategory, product.ProductName);
                if (prevProduct != null)
                {
                    response.Success = false;
                    response.Message = "Product already exits.";

                    return response;
                }

                var newProduct = new Product
                {
                    PartitionKey = product.ProductCategory,
                    RowKey = product.ProductName,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice,
                    UnitsInStock = product.UnitsInStock,
                    CreateDateTime = DateTime.Now
                };

                await Add(newProduct);

                response.Success = true;
                response.Message = "Product added successfully.";

                return response;

            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = $"There was an error adding the product.";
                return response;
            }
        }

        public async Task<ServiceResponse<string>> DeleteProduct(string productCategory, string productName)
        {
            var response = new ServiceResponse<string>();

            try
            {
                await DeleteSingle(productCategory, productCategory);

                response.Success = true;
                response.Message = "Product deleted successfully.";
                return response;

            }

            catch (Exception exception)
            {
                response.Success = false;
                response.Message = $"There was an error deleting the product: {exception}";
                return response;
            }
        }
        
        public async Task<bool> ProductExist(string productName)
        {
            var query = new TableQuery<Product>
            {
                FilterString = $"RowKey eq '{productName}'"
            };

            var entities = new List<Product>();

            TableContinuationToken token = null;
            do
            {
                var resultSegment = await Table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                entities.AddRange(resultSegment.Results);

            } while (token != null);

            return entities.Count > 0;
        }
    }
}
