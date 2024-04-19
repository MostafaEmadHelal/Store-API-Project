using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Specification.Order
{
    public class OrderWithPaymentIntentSpecification : BaseSpecification<Data.Entities.OrderEntities.Order>
    {
        public OrderWithPaymentIntentSpecification(string? paymentIntentId)
            : base(order => order.PaymentIntentId == paymentIntentId)
        {
         
        }
    }
}
