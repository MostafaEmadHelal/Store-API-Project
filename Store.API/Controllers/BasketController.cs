using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Service.Services.BasketService;
using Store.Service.Services.BasketService.Dtos;

namespace Store.API.Controllers
{
    public class BasketController : BaseController 
    {
        private readonly IBasketService baskerService;

        public BasketController(IBasketService baskerService)
        {
            this.baskerService = baskerService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasketDto>> GetBasketById(string id)
            => Ok(await baskerService.GetBasketAsync(id));

        [HttpPost]
        public async Task<ActionResult<CustomerBasketDto>> UpdateBasketAsync(CustomerBasketDto basket)
            => Ok(await baskerService.UpdateBasketAsync(basket));

        [HttpDelete]
        public async Task<ActionResult> DeleteBasketAsync(string id)
            => Ok(await baskerService.DeleteBasketAsync(id));
    }
    
}
