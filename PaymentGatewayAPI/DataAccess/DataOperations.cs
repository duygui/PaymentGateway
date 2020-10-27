using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using NPoco;
using PaymentGatewayAPI.DataAccess.Entities;
using PaymentGatewayAPI.Helpers;
using PaymentModels;
using PaymentModels.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PaymentGatewayAPI.DataAccess
{
    public class DataOperations : IDataOperations
    {
        private readonly IConfiguration _configuration;
        private List<TenantEntity> tenants;

        public DataOperations(IConfiguration configuration) => _configuration = configuration;

        public ResponseLogEntity GetTrackingInfo(Guid trackingNumber)
        {
            ResponseLogEntity trackingInfo;
            var authorizedTenant = GetTenantByApiKey();
            using (IDatabase db = GetDatabase())
                trackingInfo = db.Fetch<ResponseLogEntity>().FirstOrDefault(p => p.TrackingNumber == trackingNumber && p.TenantId == authorizedTenant.Id);
            if (trackingInfo != null) trackingInfo.CardNumber = trackingInfo.CardNumber?.Masked();
            return trackingInfo;
        }

        public void LogRequest(Guid trackingNumber, PaymentModel paymentModel)
        {
            var requestLog = new RequestLogEntity()
            {
                TrackingNumber = trackingNumber,
                RequestDate = DateTime.Now,
                RequestData = JsonConvert.SerializeObject(paymentModel),
                TenantId = GetTenantByApiKey().Id
            };

            using IDatabase db = GetDatabase();
            db.Save(requestLog);
        }

        public void SaveTrackingInfo(Guid trackingNumber, ResponseModel<Guid> resultObject, PaymentModel model)
        {
            var responseLog = new ResponseLogEntity()
            {
                TrackingNumber = trackingNumber,
                ExternalReference = resultObject.Data,
                ProcessingDate = DateTime.Now,
                Success = resultObject.Success,
                ResponsePhase = resultObject.Notification,
                StatusCode = resultObject.StatusCode,
                Amount = model.Amount,
                CardNumber = model.CreditCartInfo?.CardNumber ?? string.Empty,
                TenantId = GetTenantByApiKey().Id
            };

            using IDatabase db = GetDatabase();
            db.Save(responseLog);
        }

        public List<TenantEntity> GetTenants()
        {
            if (tenants == null || !tenants.Any())
            {
                using IDatabase db = GetDatabase();
                tenants = db.Fetch<TenantEntity>();
            }
            return tenants;
        }

        private TenantEntity GetTenantByApiKey()
        {
            Guid.TryParse(Thread.CurrentPrincipal.Identity.Name, out Guid apiKey);
            return GetTenants().FirstOrDefault(p => Equals(p.ApiKey, apiKey));
        }

        public EAcquiringBanks GetAcquiringBank()
        {
            Guid.TryParse(Thread.CurrentPrincipal.Identity.Name, out Guid apiKey);
            var acquiringBankId = GetTenants().FirstOrDefault(p => Equals(p.ApiKey, apiKey))?.AcquiringBankId;
            if (!Enum.TryParse(acquiringBankId.ToString(), out EAcquiringBanks acquiringBank))
                throw new Exception("Acquiring bank info is not valid! Please check your configurations");
            return acquiringBank;
        }

        private Database GetDatabase()
        {
            string connectionString = _configuration.GetConnectionString("Default");
            var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            return new Database(conn);
        }
    }
}
