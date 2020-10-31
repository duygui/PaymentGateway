using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentGatewayAPI.Filters;
using PaymentGatewayAPI.Repositories;
using PaymentModels;

namespace PaymentGatewayAPI.Controllers
{
    [ApiKeyAuthentication]
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository) =>
            _paymentRepository = paymentRepository;

        /// <summary>
        /// Sends given payment information to the acquiring bank.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Pay([FromBody] PaymentModel paymentModel) =>
             await _paymentRepository.Pay(paymentModel);


        /// <summary>
        /// Gets the payment detail with the given tracking number.
        /// </summary>
        /// <param name="trackingNumber">Payment tracking number provided from gateway after payment operation</param>
        [HttpGet]
        public IActionResult GetPaymentDetail(Guid trackingNumber) =>
             _paymentRepository.GetPaymentDetail(trackingNumber);

    }
}
