using Store.Data.Entities.OrderEntities;
using Store.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.OrderService.Dtos
{
    public class OrderResultDto
    {
        public Guid Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } 
        public AddressDto ShippingAddress { get; set; }
        public string DeliveryMethodName { get; set; }
        public OrderPaymentStatus OrderPaymentStatus { get; set; } 
        public IReadOnlyList<OrderItemDto> OrderItmes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal Total { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? BasketId { get; set; }

    }
}
