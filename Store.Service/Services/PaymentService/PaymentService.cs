using AutoMapper;
using Microsoft.Extensions.Configuration;
using Store.Data.Entities;
using Store.Data.Entities.OrderEntities;
using Store.Repository.Interfaces;
using Store.Repository.Specification.Order;
using Store.Service.Services.BasketService;
using Store.Service.Services.BasketService.Dtos;
using Store.Service.Services.OrderService.Dtos;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = Store.Data.Entities.Product;

namespace Store.Service.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration configuration;
        private readonly IBasketService basketService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PaymentService(IConfiguration configuration,
            IBasketService basketService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.configuration = configuration;
            this.basketService = basketService;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<CustomerBasketDto> CreateOrUpdatePaymentIntentForExistingOrder(CustomerBasketDto basket)
        {
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            if (basket == null)
                throw new Exception("Basket Is Null");

            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod, int>().GetByIdAsync(basket.DeliveryMethodId.Value);

            var shippingPrice = deliveryMethod.Price;

            foreach(var item in basket.BasketItems)
            {
                var product = await unitOfWork.Repository<Product, int>().GetByIdAsync(item.ProductId);

                if(item.Price != product.Price)
                    item.Price = product.Price;
            }

            var service = new PaymentIntentService();

            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                paymentIntent = await service.CreateAsync(options);

                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;

            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),
                };

                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            await basketService.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<CustomerBasketDto> CreateOrUpdatePaymentIntentForNewOrder(string basketId)
        {
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
            
            var basket = await basketService.GetBasketAsync(basketId);
            if (basket == null)
                throw new Exception("Basket Is Null");

            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod, int>().GetByIdAsync(basket.DeliveryMethodId.Value);

            var shippingPrice = deliveryMethod.Price;

            foreach (var item in basket.BasketItems)
            {
                var product = await unitOfWork.Repository<Product, int>().GetByIdAsync(item.ProductId);

                if (item.Price != product.Price)
                    item.Price = product.Price;
            }

            var service = new PaymentIntentService();

            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                paymentIntent = await service.CreateAsync(options);

                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;

            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),
                };

                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            await basketService.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<OrderResultDto> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var specs = new OrderWithPaymentIntentSpecification(paymentIntentId);

            var order = await unitOfWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if (order is null)
                throw new Exception("Order Does Not Exist");

            order.OrderPaymentStatus = OrderPaymentStatus.Failed;

            unitOfWork.Repository<Order, Guid>().Update(order);

            await unitOfWork.CompleteAsync();

            var mappedOrder = mapper.Map<OrderResultDto>(order);

            return mappedOrder;
        }

        public async Task<OrderResultDto> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var specs = new OrderWithPaymentIntentSpecification(paymentIntentId);

            var order = await unitOfWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if (order is null)
                throw new Exception("Order Does Not Exist");

            order.OrderPaymentStatus = OrderPaymentStatus.Received;

            unitOfWork.Repository<Order, Guid>().Update(order);

            await unitOfWork.CompleteAsync();

            await basketService.DeleteBasketAsync(order.BasketId);

            var mappedOrder = mapper.Map<OrderResultDto>(order);

            return mappedOrder;
        }
    }
}
