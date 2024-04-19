using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Service.Services.BasketService.Dtos;
using Store.Service.Services.OrderService.Dtos;
using Store.Service.Services.PaymentService;
using Stripe;

namespace Store.API.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentController> logger;
        private const string endpointSecret = "whsec_4f3ae48381e7a50f7ffde292b036bf971be07991a7a9781978159f334f106a73";
        public PaymentController(
            IPaymentService paymentService,
            ILogger<PaymentController> logger)
        {
            this.paymentService = paymentService;
            this.logger = logger;
        }

        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntentForExistingOrder(CustomerBasketDto input)
          => Ok(await paymentService.CreateOrUpdatePaymentIntentForExistingOrder(input));

        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntentForNewOrder(string basketId)
          => Ok(await paymentService.CreateOrUpdatePaymentIntentForNewOrder(basketId));

            [HttpPost("webhook")]
            public async Task<IActionResult> Index()
            {
                 var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                 try
                {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                PaymentIntent paymentIntent;
                OrderResultDto order;
                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                    logger.LogInformation("Payment Failed : ", paymentIntent.Id);
                    order = await paymentService.UpdateOrderPaymentFailed(paymentIntent.Id);
                    logger.LogInformation("Order Updated To Payment Failed", order.Id);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                    logger.LogInformation("Payment Succeeded : ", paymentIntent.Id);
                    order = await paymentService.UpdateOrderPaymentSucceeded(paymentIntent.Id);
                    logger.LogInformation("Order Updated To Payment Succeeded", order.Id);
                }
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
                }
                catch (StripeException e)
                {
                    return BadRequest();
                }
            }
        //}
    }    
}
