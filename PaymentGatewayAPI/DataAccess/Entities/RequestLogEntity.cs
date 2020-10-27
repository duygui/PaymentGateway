using Newtonsoft.Json;
using NPoco;
using System;
using System.Runtime.Serialization;

namespace PaymentGatewayAPI.DataAccess.Entities
{
    [Serializable]
    [TableName("RequestLog")]
    [PrimaryKey("TrackingNumber", AutoIncrement = false)]
    public class RequestLogEntity
    {
        [Column("TrackingNumber")]
        [DataMember]
        public Guid TrackingNumber { get; set; }
       
        [Column("RequestDate")]
        [DataMember]
        public DateTime RequestDate { get; set; }
       
        [Column("RequestData")]
        [DataMember]
        public string RequestData { get; set; }

        [JsonIgnore]
        [Column("TenantId")]
        [DataMember]
        public int TenantId { get; set; }
    }
}
