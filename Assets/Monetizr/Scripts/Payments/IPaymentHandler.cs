using Monetizr.Dto;

namespace Monetizr.Payments
{
    public interface IPaymentHandler
    {
        void GetResponse(PaymentStatusResponse response);
        bool IsPolling();
        void CancelPayment();
        void Process();
    }
}