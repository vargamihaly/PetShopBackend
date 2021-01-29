using System.Threading.Tasks;
using core.DTO;
using core.Interfaces;
using core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[EnableCors("CorsPolicy")]
    public class ProductsController : Controller
    {
        private ProductService ProductService { get; }
        private IHostEnvironment Environment { get; }

        public ProductsController(IProductServiceFactory productServiceFactory,
            IHostEnvironment environment)
        {
            ProductService = productServiceFactory.GetProductService();
            Environment = environment;
        }


        [HttpGet("GetProducts")]
        [ActionName("Products")]
        public async Task<IActionResult> GetEveryProduct()
        {
            var response = await ProductService.GetAllProduct();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpGet("GetProductsInCategory")]
        [ActionName("Products")]
        public async Task<IActionResult> GetEveryProductInCategory(string productCategory)
        {
            var response = await ProductService.GetAllProductWithinCategory(productCategory);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpGet("GetProduct")]
        [ActionName("GetProduct")]
        public async Task<IActionResult> GetProduct(string productCategory, string productName)
        {
            var response = await ProductService.GetProduct(productCategory, productName);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpPost("AddProduct")]
        [ActionName("AddProduct")]
        public async Task<IActionResult> AddProduct(ProductDto product)
        {
            var response = await ProductService.AddProduct(product);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        

        [HttpDelete("DeleteProduct")]
        [ActionName("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(string productCategory, string productName)
        {
            var response = await ProductService.DeleteProduct(productCategory, productName);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
