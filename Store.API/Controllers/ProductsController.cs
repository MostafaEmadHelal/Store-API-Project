using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.API.Helper;
using Store.Repository.Specification.Product;
using Store.Service.HandleResponses;
using Store.Service.Helper;
using Store.Service.Services.ProductService;
using Store.Service.Services.ProductService.Dtos;

namespace Store.API.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        [Cache(90)]
        public async Task<ActionResult<IReadOnlyList<BrandTypeDetailsDto>>> GetAllBrands()
         => Ok(await productService.GetAllBrandsAsync());

        [HttpGet]
        [Cache(90)]
        public async Task<ActionResult<IReadOnlyList<BrandTypeDetailsDto>>> GetAllTypes()
        => Ok(await productService.GetAllTypesAsync());

        [HttpGet]
        [Cache(90)]
        public async Task<ActionResult<PaginatedResultDto<ProductDetailsDto>>> GetAllProducts([FromQuery] ProductSpecification input)
        => Ok(await productService.GetAllProductsAsync(input));

        [HttpGet]
        [Cache(90)]
        public async Task<ActionResult<ProductDetailsDto>> GetProduct(int? id)
        { if (id is null)
                return BadRequest(new Response(400, "Id Is Null"));

          var product = await productService.GetProductByIdAsync(id);

          if(product is null)
             return NotFound(new Response(404));
          return Ok(product);
        }
    }
}
