using Microsoft.AspNetCore.Mvc;
using PaymentModels;
using System;
using System.Threading.Tasks;

namespace PaymentGatewayAPI.Repositories
{
    public interface IPaymentRepository
    {
        Task<IActionResult> Pay(PaymentModel paymentModel);

        IActionResult GetPaymentDetail(Guid trackingNumber);
    }
}
