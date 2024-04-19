using AutoMapper;
using Store.Data.Entities;
using Store.Data.Entities.OrderEntities;
using Store.Repository.BasketRepository;
using Store.Repository.Interfaces;
using Store.Repository.Specification.Order;
using Store.Service.Services.BasketService;
using Store.Service.Services.OrderService.Dtos;
using Store.Service.Services.PaymentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBasketService basketService;
        private readonly IMapper mapper;
        private readonly IPaymentService paymentService;

        public OrderService(IUnitOfWork unitOfWork,
            IBasketService basketService,
            IMapper mapper,
            IPaymentService paymentService)
        {
            this.unitOfWork = unitOfWork;
            this.basketService = basketService;
            this.mapper = mapper;
            this.paymentService = paymentService;
        }
        public async Task<OrderResultDto> CreateOrderAsync(OrderDto input)
        {
            // Get Basket
            var basket = await basketService.GetBasketAsync(input.BasketId);

            if (basket is null)
                throw new Exception("Basket Doesn't Exist");

            // Fill OrderItems From Basket Items
            var orderitems = new List<OrderItemDto>();

            foreach (var basketItem in basket.BasketItems)
            {
                var productItem = await unitOfWork.Repository<Product, int>().GetByIdAsync(basketItem.ProductId);

                if (productItem is null)
                    throw new Exception($"Product With Id : {basketItem.ProductId} Doesn't Exist");

                var itemOrdered = new ProductItemOrdered
                {
                    ProductItemId = productItem.Id,
                    ProductName = productItem.Name,
                    PictureUrl = productItem.PictureUrl
                };

                var orderItem = new OrderItem
                {
                    Price = productItem.Price,
                    Quantity = basketItem.Quantity,
                    ItemOrdered = itemOrdered,
                };

                var mapperOrderItem = mapper.Map<OrderItemDto>(orderItem);

                orderitems.Add(mapperOrderItem);
            }

            // Get Delivery Method
            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod, int>().GetByIdAsync(input.DeliveryMethodId);

            if(deliveryMethod is null)
                    throw new Exception("Delivery Method Is Not Provided");

            // Calculate Subtotal
            var subtotal = orderitems.Sum(item => item.Quantity * item.Price);

            // To Do => Check If Order Exists

            var specs = new OrderWithPaymentIntentSpecification(basket.PaymentIntentId);

            var existingOrder = await unitOfWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if (existingOrder != null)
            {
                unitOfWork.Repository<Order, Guid>().Delete(existingOrder);
                await paymentService.CreateOrUpdatePaymentIntentForExistingOrder(basket);
            }
            else
            {
                await paymentService.CreateOrUpdatePaymentIntentForNewOrder(basket.Id);
            }

            // Create Order
            var mappedShippingAddress = mapper.Map<ShippingAddress>(input.ShippingAddress);

            var mappedOrderItems = mapper.Map<List<OrderItem>>(orderitems);

            var order = new Order
            {
                DeliveryMethodId = deliveryMethod.Id,
                ShippingAddress = mappedShippingAddress,
                BuyerEmail = input.BuyerEmail,
                OrderItems = mappedOrderItems,
                SubTotal = subtotal,
                BasketId = basket.Id,
                PaymentIntentId = basket.PaymentIntentId
            }; 

            await unitOfWork.Repository<Order, Guid>().AddAsync(order);

            await unitOfWork.CompleteAsync();

            var mappedOrder = mapper.Map<OrderResultDto>(order);

            return mappedOrder;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetAllDeliveryMethodsAsync()
            => await unitOfWork.Repository<DeliveryMethod, int>().GetAllAsync();
        

        public async Task<IReadOnlyList<OrderResultDto>> GetAllOrdersForUserAsync(string buyerEmail)
        {
            var specs = new OrderWithItemsSpecification(buyerEmail);

            var orders = await unitOfWork.Repository<Order, Guid>().GetAllWithSpecificationAsync(specs);

            if (orders is { Count: <= 0 })
                throw new Exception("You Do Not Have Any Orders Yet");

            var mappedOrders = mapper.Map<List<OrderResultDto>>(orders);

            return mappedOrders;
        }

        public async Task<OrderResultDto> GetOrderByIdAsync(Guid id, string buyerEmail)
        {
            var specs = new OrderWithItemsSpecification(id,buyerEmail);

            var order = await unitOfWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if(order is null)
                throw new Exception($"There Is No Order With Id : {id}");

            var mappedOrder = mapper.Map<OrderResultDto>(order);



            return mappedOrder;
        }
    }
}
