using AutoMapper;
using Store.Repository.BasketRepository;
using Store.Repository.BasketRepository.Models;
using Store.Service.Services.BasketService.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.BasketService
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketService(IBasketRepository basketRepository, IMapper mapper)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }
        public async Task<bool> DeleteBasketAsync(string basketId)
         => await basketRepository.DeleteBasketAsync(basketId);


        public async Task<CustomerBasketDto> GetBasketAsync(string basketId)
        {
            var basket = await basketRepository.GetBasketAsync(basketId);

            if(basket is null)
                return new CustomerBasketDto();

            var mappedBasket = mapper.Map<CustomerBasketDto>(basket);

            return mappedBasket;
        }

        public async Task<CustomerBasketDto> UpdateBasketAsync(CustomerBasketDto basket)
        {
            var customerBasket = mapper.Map<CustomerBasket>(basket);

            var updatedBasket = await basketRepository.UpdateBasketAsync(customerBasket);

            var mappedCustomerBasket = mapper.Map<CustomerBasketDto>(updatedBasket);

            return mappedCustomerBasket;
        }
    }
}
