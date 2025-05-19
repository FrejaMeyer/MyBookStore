using Microsoft.AspNetCore.Mvc;
using Payment.Services;
using Payment.Models;
using Dapr;

namespace Payment.Controllers
{
    [ApiController]
    [Route("payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentServices _paymentServices;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentServices paymentServices, ILogger<PaymentController> logger)
        {
            _paymentServices = paymentServices;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest paymentRequest)
        {
            _logger.LogInformation("ProcessPayment hit for OrderId: {OrderId}", paymentRequest.OrderId);

            var result = await _paymentServices.ProcessPaymentAsync(paymentRequest);
            return Ok(result);
        }

        [Topic("bookpubsub", "payment-request")]
        [HttpPost("method/payment-request")]
        public async Task<IActionResult> OnPaymentRequest([FromBody] PaymentRequest paymentRequest)
        {
            var result = await _paymentServices.ProcessPaymentAsync(paymentRequest);
            return Ok(result);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(string orderId)
        {
            var result = await _paymentServices.GetPaymentStatusAsync(orderId);
            if (result == null)
            {
                return NotFound($"Payment status for OrderId: {orderId} not found.");
            }
            return Ok(result);
        }
    }
}
