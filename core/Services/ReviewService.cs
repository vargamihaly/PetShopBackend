using Microsoft.WindowsAzure.Storage.Table;
using core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using core.DTO;

namespace core.Services
{
    public class ReviewService : AzureTableService<Review>
    {
        private const int MaximumNumberOfCharactersForReviews = 500;

        public ReviewService(CloudTable reviewTable) : base(reviewTable)
        {
        }

        public async Task<ServiceResponse<IEnumerable<ReviewDto>>> GetNewestReviews(string productName,
            int? numberOfReviews = 5)
        {
            var response = new ServiceResponse<IEnumerable<ReviewDto>>();


            try
            {
                var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, productName);

                var query = new TableQuery<Review>().Where(filter).Take(numberOfReviews);

                var reviews = new List<Review>();

                TableContinuationToken token = null;
                do
                {
                    var resultSegment = await Table.ExecuteQuerySegmentedAsync(query, token);
                    token = resultSegment.ContinuationToken;

                    reviews.AddRange(resultSegment.Results);

                } while (token != null);

                if (reviews.Count == 0)
                {
                    response.Success = false;
                    response.Message = "There are no reviews for this product";
                    return response;
                }

                response.Success = true;
                response.Data = reviews.OrderBy(x => x.RowKey).Select(x => new ReviewDto
                {
                    Comment = x.Comment,
                    UserName = x.UserName
                });
                
                return response;

            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = $"There was an error getting the reviews.";
                return response;
            }
        }

        public async Task<IEnumerable<Review>> GetAllReviews(string productName)
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, productName);

            var query = new TableQuery<Review>().Where(filter);

            var entities = new List<Review>();

            TableContinuationToken token = null;
            do
            {
                var resultSegment = await Table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                entities.AddRange(resultSegment.Results);

            } while (token != null);

            return entities;
        }

        public async Task<ServiceResponse<Tuple<IEnumerable<Review>, string, string>>> GetReviewsWithPaging(
            string productName, int numberOfReviews, string nextPartitionKey, string nextRowKey)
        {
            var response = new ServiceResponse<Tuple<IEnumerable<Review>, string, string>>();

            try
            {
                var token = new TableContinuationToken
                {
                    NextPartitionKey = nextPartitionKey,
                    NextRowKey = nextRowKey,
                    NextTableName = "Reviews"
                };

                var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, productName);

                var query = new TableQuery<Review>().Where(filter).Take(numberOfReviews);

                var entities = new List<Review>();

                var resultSegment = await Table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                entities.AddRange(resultSegment.Results);

                response.Success = true;
                response.Data =
                    new Tuple<IEnumerable<Review>, string, string>(entities, token.NextPartitionKey, token.NextRowKey);

                return response;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = $"There was an error getting the reviews.";
                return response;
            }
        }


        public async Task<ServiceResponse<string>> AddReview(string rowKeyOfLastReview, ReviewDto review)
        {
            var response = new ServiceResponse<string>();


            var latestRowKey = GetNewestReviewRowKey(review.ProductName).Result;

            if (!string.IsNullOrEmpty(latestRowKey) && latestRowKey != rowKeyOfLastReview)
            {
                response.Success = false;
                response.Message = "To add a new review, the client must provide the last review of the product.";
                return response;
            }
            
            if (review.Comment.Length > MaximumNumberOfCharactersForReviews)
            {
                response.Success = false;
                response.Message = $"The comment can be maximum {MaximumNumberOfCharactersForReviews} characters long.";
                return response;
            }

            try
            {
                var invertedTimeKey = $"{DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks:D19}";
                var newReview = new Review
                {
                    PartitionKey = review.ProductName,
                    RowKey = invertedTimeKey,
                    UserName = review.UserName,
                    Comment = review.Comment,
                    CreateDateTime = DateTime.Now
                };

                await Add(newReview);

                response.Success = true;
                response.Message = "Review added successfully.";
                response.Data = invertedTimeKey;
                return response;

            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = $"There was an error adding the review.";
                return response;
            }
        }
        

        public async Task<ServiceResponse<string>> DeleteReview(string productName, string invertedRowKey)
        {
            var response = new ServiceResponse<string>();
            
            try
            {
                await DeleteSingle(productName, invertedRowKey);

                response.Success = true;
                response.Message = $"Review deleted successfully";
                return response;

            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = $"There was an error adding the review.";
                return response;
            }
        }

        private async Task<string> GetNewestReviewRowKey(string productName)
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, productName);

            var query = new TableQuery<Review>().Where(filter).Take(1);

            TableContinuationToken token = null;

            var resultSegment = await Table.ExecuteQuerySegmentedAsync(query, token);

            var review = resultSegment.Results.FirstOrDefault();

            return review?.RowKey;
        }
    }

}