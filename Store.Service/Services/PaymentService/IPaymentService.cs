using Store.Service.Services.BasketService.Dtos;
using Store.Service.Services.OrderService.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<CustomerBasketDto> CreateOrUpdatePaymentIntentForExistingOrder(CustomerBasketDto input);
        Task<CustomerBasketDto> CreateOrUpdatePaymentIntentForNewOrder(string basketId);
        Task<OrderResultDto> UpdateOrderPaymentSucceeded(string paymentIntentId);
        Task<OrderResultDto> UpdateOrderPaymentFailed(string paymentIntentId);

    }
}
