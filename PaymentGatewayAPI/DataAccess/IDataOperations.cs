using PaymentGatewayAPI.DataAccess.Entities;
using PaymentModels;
using PaymentModels.Enumeration;
using System;
using System.Collections.Generic;

namespace PaymentGatewayAPI.DataAccess
{
    public interface IDataOperations
    {
        void SaveTrackingInfo(Guid trackingNumber, ResponseModel<Guid> resultObject, PaymentModel model);

        void LogRequest(Guid trackingNumber, PaymentModel paymentModel);

        ResponseLogEntity GetTrackingInfo(Guid trackingNumber);

        List<TenantEntity> GetTenants();

        EAcquiringBanks GetAcquiringBank();
    }
}
