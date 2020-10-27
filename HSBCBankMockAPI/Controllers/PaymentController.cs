using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentModels;

namespace HSBCBankMockAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }


        [HttpPost]
        public IActionResult Pay(PaymentModel model)
        {
            return Ok(new ResponseModel<PaymentModel>() { Success = true, TrackingNumber = Guid.NewGuid(), Data = model });
        }
    }
}
