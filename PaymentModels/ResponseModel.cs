using System;
using System.Text.Json.Serialization;

namespace PaymentModels
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            Success = true;
        }

        public bool Success { get; set; }

        public string Notification { get; set; }

        public Guid TrackingNumber { get; set; }

        [JsonIgnore]
        public int StatusCode { get; set; }
    }

    public class ResponseModel<T> : ResponseModel
    {
        public T Data { get; set; }
    }
}
