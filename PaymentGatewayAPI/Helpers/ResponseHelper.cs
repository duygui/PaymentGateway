using PaymentModels;
using System;
using System.Net;

namespace PaymentGatewayAPI.Helpers
{
    public static class ResponseHelper
    {
        public static ResponseModel<Guid> GetSuccessResponse(Guid trackingNumber, Guid externalReference, HttpStatusCode statusCode = HttpStatusCode.OK, string notification = null)
            => new ResponseModel<Guid>()
            {
                Success = true,
                TrackingNumber = trackingNumber,
                Data = externalReference,
                Notification = notification ?? "OK",
                StatusCode = (int)statusCode
            };

        public static ResponseModel<Guid> GetFailureResponse(Guid trackingNumber, Guid externalReference, HttpStatusCode statusCode, string notification)
           => new ResponseModel<Guid>()
           {
               Success = false,
               TrackingNumber = trackingNumber,
               Data = externalReference,
               Notification = notification,
               StatusCode = (int)statusCode
           };
    }
}
