using NPoco;
using System;
using System.Runtime.Serialization;

namespace PaymentGatewayAPI.DataAccess.Entities
{
    [Serializable]
    [TableName("Tenants")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class TenantEntity
    {
        [Column("Id")]
        [DataMember]
        public int Id { get; set; }

        [Column("Name")]
        [DataMember]
        public string Name { get; set; }

        [Column("ApiKey")]
        [DataMember]
        public Guid ApiKey { get; set; }

        [Column("AcquiringBankId")]
        [DataMember]
        public int AcquiringBankId { get; set; }
    }
}
