using FluentMigrator;
using PaymentGatewayAPI.DataAccess.Entities;
using PaymentModels.Enumeration;
using System;

namespace PaymentGatewayAPI.DataMigration.DbScripts
{
    [Migration(20201025)]
    public class StartupScripts : Migration
    {
        public override void Down()
        {
            Delete.Table("RequestLog");
            Delete.Table("ResponseLog");
            Delete.Table("Tenants");
        }

        public override void Up()
        {
            if (!Schema.Table("Tenants").Exists())
            {
                Create.Table("Tenants")
                     .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                     .WithColumn("Name").AsString().NotNullable()
                     .WithColumn("ApiKey").AsGuid().NotNullable()
                     .WithColumn("AcquiringBankId").AsInt32().NotNullable();

                //Insert mock data for mock banks and mock tenants
                Insert.IntoTable("Tenants").Row(new TenantEntity() { Id = 1, Name = "MockTenant1", ApiKey = Guid.Parse("034315C9-6BDE-41A2-AAE1-0A241FA8F5CE"), AcquiringBankId = (int)EAcquiringBanks.HSBCBank });
                Insert.IntoTable("Tenants").Row(new TenantEntity() { Id = 2, Name = "MockTenant2", ApiKey = Guid.Parse("234FA67C-598F-4FBE-BE5F-D855DC192F22"), AcquiringBankId = (int)EAcquiringBanks.INGBank });
            }

            if (!Schema.Table("RequestLog").Exists())
                Create.Table("RequestLog")
                    .WithColumn("TrackingNumber").AsGuid().PrimaryKey()
                    .WithColumn("RequestDate").AsDateTime().NotNullable()
                    .WithColumn("RequestData").AsString().NotNullable()
                    .WithColumn("TenantId").AsInt32().NotNullable().ForeignKey("Tenants", "Id");

            if (!Schema.Table("ResponseLog").Exists())
                Create.Table("ResponseLog")
                    .WithColumn("TrackingNumber").AsGuid().PrimaryKey()
                    .WithColumn("ExternalReference").AsGuid().NotNullable()
                    .WithColumn("ProcessingDate").AsDateTime().NotNullable()
                    .WithColumn("Success").AsBoolean().NotNullable()
                    .WithColumn("StatusCode").AsInt32().NotNullable()
                    .WithColumn("ResponsePhase").AsString().NotNullable()
                    .WithColumn("CardNumber").AsString().NotNullable()
                    .WithColumn("Amount").AsDecimal().NotNullable()
                    .WithColumn("TenantId").AsInt32().NotNullable().ForeignKey("Tenants", "Id");
        }
    }
}
