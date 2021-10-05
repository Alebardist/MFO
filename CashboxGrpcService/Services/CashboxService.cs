using Grpc.Core;
using System.Threading.Tasks;

namespace CashboxGrpcService.Services
{
    public class CashboxService : Cashbox.CashboxBase
    {
        public override Task<SendMoneyReply> SendMoney(SendMoneyRequest request, ServerCallContext context)
        {
            //request with card number unused

            var reply = new SendMoneyReply()
            {
                OperationResult = "OK"
            };

            return Task.FromResult(reply);
        }
    }
}
