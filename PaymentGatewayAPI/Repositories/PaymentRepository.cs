using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.DataAccess;
using PaymentGatewayAPI.DataAccess.Entities;
using PaymentGatewayAPI.Helpers;
using PaymentModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PaymentGatewayAPI.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDataOperations _dataOperations;
        readonly IPaymentFactory _paymentFactory;
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(IDataOperations dataOperations, IPaymentFactory paymentFactory, ILogger<PaymentRepository> logger)
        {
            _dataOperations = dataOperations;
            _paymentFactory = paymentFactory;
            _logger = logger;
        }

        public IActionResult GetPaymentDetail(Guid trackingNumber)
        {
            _logger.LogInformation($"Starting GetPaymentDetail method for {trackingNumber}");
            var paymentResult = _dataOperations.GetTrackingInfo(trackingNumber);
            return paymentResult != null
                ? new ObjectResult(new ResponseModel<ResponseLogEntity>() { Success = true, TrackingNumber = trackingNumber, Data = paymentResult, Notification = "OK" }) { StatusCode = (int)HttpStatusCode.OK }
                : new ObjectResult(new ResponseModel<ResponseLogEntity>() { Success = false, TrackingNumber = trackingNumber, Notification = "Payment info not found with the given identifier" }) { StatusCode = (int)HttpStatusCode.NotFound };
        }

        public async Task<IActionResult> Pay(PaymentModel paymentModel)
        {
            _logger.LogInformation("Starting Pay method");
            var trackingNumber = Guid.NewGuid();
            ResponseModel<Guid> resultObject = new ResponseModel<Guid>() { };
            try
            {
                _dataOperations.LogRequest(trackingNumber, paymentModel);

                resultObject = _paymentFactory.ValidateModelForPaymentOption(paymentModel, trackingNumber);
                if (!resultObject.Success)
                    return new ObjectResult(resultObject) { StatusCode = (int)HttpStatusCode.BadRequest };

                resultObject = await DoPaymentOperation(paymentModel, trackingNumber);
                return resultObject.Success
                    ? new ObjectResult(resultObject) { StatusCode = (int)HttpStatusCode.Created }
                    : new ObjectResult(resultObject) { StatusCode = (int)HttpStatusCode.BadGateway };
            }
            catch (Exception e)
            {
                _logger.LogError($"Pay method failed! {e}");
                resultObject = ResponseHelper.GetFailureResponse(trackingNumber, Guid.Empty, HttpStatusCode.BadRequest, e.Message);
                return new ObjectResult(resultObject) { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            finally
            {
                _dataOperations.SaveTrackingInfo(trackingNumber, resultObject, paymentModel);
            }
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
