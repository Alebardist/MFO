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
                ErrorMessage = "",
                Result = SendMoneyReply.Types.operationResult.Ok
            };

            return Task.FromResult(reply);
        }
    }
}
