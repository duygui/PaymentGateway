using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.DataAccess;
using PaymentGatewayAPI.DataAccess.Entities;
using PaymentGatewayAPI.Filters;
using PaymentGatewayAPI.Helpers;
using PaymentModels;

namespace PaymentGatewayAPI.Controllers
{
    [ApiKeyAuthentication]
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {

        private readonly ILogger<PaymentController> _logger;
        private readonly IDataOperations _dataOperations;
        readonly IPaymentFactory _paymentFactory;

        public PaymentController(ILogger<PaymentController> logger, IDataOperations dataOperations, IPaymentFactory paymentFactory)
        {
            _logger = logger;
            _dataOperations = dataOperations;
            _paymentFactory = paymentFactory;
        }

        /// <summary>
        /// Sends given payment information to the acquiring bank.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Pay([FromBody] PaymentModel paymentModel)
        {
            _logger.LogInformation("Starting Pay method");
            var trackingNumber = Guid.NewGuid();
            ResponseModel<Guid> resultObject = new ResponseModel<Guid>() { };
            try
            {
                _dataOperations.LogRequest(trackingNumber, paymentModel);

                resultObject = _paymentFactory.ValidateModelForPaymentOption(paymentModel, trackingNumber);
                if (!resultObject.Success)
                    return BadRequest(resultObject);

                resultObject = await DoPaymentOperation(paymentModel, trackingNumber);
                return resultObject.Success
                    ? Created(string.Empty, resultObject)
                    : new ObjectResult(resultObject) { StatusCode = (int)HttpStatusCode.BadGateway };
            }
            catch (Exception e)
            {
                _logger.LogError($"Pay method failed! {e}");
                resultObject = ResponseHelper.GetFailureResponse(trackingNumber, Guid.Empty, System.Net.HttpStatusCode.BadRequest, e.Message);
                return BadRequest(resultObject);
            }
            finally
            {
                _dataOperations.SaveTrackingInfo(trackingNumber, resultObject, paymentModel);
            }
        }

        /// <summary>
        /// Gets the payment detail with the given tracking number.
        /// </summary>
        /// <param name="trackingNumber">Payment tracking number provided from gateway after payment operation</param>
        [HttpGet]
        public IActionResult GetPaymentDetail(Guid trackingNumber)
        {
            _logger.LogInformation("Starting GetPaymentDetail method");
            var paymentResult = _dataOperations.GetTrackingInfo(trackingNumber);
            return paymentResult != null
                ? Ok(new ResponseModel<ResponseLogEntity>() { Success = true, TrackingNumber = trackingNumber, Data = paymentResult, Notification = "OK" })
                : (IActionResult)NotFound(new ResponseModel<ResponseLogEntity>() { Success = false, TrackingNumber = trackingNumber, Notification = "Payment info not found with the given identifier" });
        }

        private async Task<ResponseModel<Guid>> DoPaymentOperation(PaymentModel paymentModel, Guid trackingNumber)
        {
            _logger.LogInformation("Starting DoPaymentOperation method");
            var paymentService = _paymentFactory.GetPaymentService();
            var requestString = paymentService.PrepareRequestData(paymentModel);
            var externalResult = await paymentService.CallDestinationPaymentService(requestString);

            if (externalResult.Success)
                return ResponseHelper.GetSuccessResponse(trackingNumber, externalResult.TrackingNumber, System.Net.HttpStatusCode.Created, externalResult.Notification);
            return ResponseHelper.GetFailureResponse(trackingNumber, externalResult.TrackingNumber, System.Net.HttpStatusCode.BadRequest, externalResult.Notification);
        }
    }
}
