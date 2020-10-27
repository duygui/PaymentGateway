using Newtonsoft.Json;
using NPoco;
using System;
using System.Runtime.Serialization;

namespace PaymentGatewayAPI.DataAccess.Entities
{
    [Serializable]
    [TableName("ResponseLog")]
    [PrimaryKey("TrackingNumber", AutoIncrement = false)]
    public class ResponseLogEntity
    {
        [Column("TrackingNumber")]
        [DataMember]
        public Guid TrackingNumber { get; set; }

        [Column("ExternalReference")]
        [DataMember]
        public Guid ExternalReference { get; set; }

        [Column("ProcessingDate")]
        [DataMember]
        public DateTime ProcessingDate { get; set; }

        [Column("Success")]
        [DataMember]
        public bool Success { get; set; }

        [Column("StatusCode")]
        [DataMember]
        public int StatusCode { get; set; }

        [Column("ResponsePhase")]
        [DataMember]
        public string ResponsePhase { get; set; }

        [Column("CardNumber")]
        [DataMember]
        public string CardNumber { get; set; }

        [Column("Amount")]
        [DataMember]
        public decimal Amount { get; set; }

        [JsonIgnore]
        [Column("TenantId")]
        [DataMember]
        public int TenantId { get; set; }
    }
}
