using System.Threading.Tasks;
using core.DTO;
using core.Interfaces;
using core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[EnableCors("CorsPolicy")]
    public class ReviewsController : Controller
    {
        private ReviewService ReviewService { get; }
        private ProductService ProductService { get; }
        private IHostEnvironment Environment { get; }

        public ReviewsController(IReviewServiceFactory reviewServiceFactory,
                                 IProductServiceFactory productServiceFactory,
                                 IHostEnvironment environment)
        {
            ReviewService = reviewServiceFactory.GetReviewService();
            ProductService = productServiceFactory.GetProductService();
            Environment = environment;
        }


        [HttpGet("GetReviews")]
        public async Task<IActionResult> GetNewestReviewsForProduct(string productName, int numberOfReviews)
        {
            var response = await ReviewService.GetNewestReviews(productName, numberOfReviews);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpGet("GetReviewsWithPaging")]
        public async Task<IActionResult> GetReviewsWithPaging(
            string productName, int numberOfReviews, string nextPartitionKey, string nextRowKey)
        {
            var response = await ReviewService.GetReviewsWithPaging(productName, numberOfReviews, nextPartitionKey, nextRowKey);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        /// <summary>
        /// Before adding a new review, we have to make sure that the client received the newest review
        /// The comment can be maximum 500 character long 
        /// </summary>
        /// <param name="rowKeyOfLastReview"></param>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview(string rowKeyOfLastReview, ReviewDto review)
        {
            var response = await ReviewService.AddReview(rowKeyOfLastReview, review);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReview")]
        public async Task<IActionResult> DeleteReview(string productName, string reviewInvertedTimeKey)
        {
            var response = await ReviewService.DeleteReview(productName, reviewInvertedTimeKey);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
