using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Microsoft.Extensions.Configuration;

using MongoDB.Driver;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashboxGrpcService
{
    public class CashboxService : Cashbox.CashboxBase
    {
        private readonly IConfiguration _configuration;

        public CashboxService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //UNDONE: method unimplemented
        public override Task<SendMoneyReply> SendMoney(SendMoneyRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();

            SendMoneyReply reply = new()
            {
                ErrorMessage = "",
                Result = SendMoneyReply.Types.operationResult.Error
            };

            return Task.FromResult(reply);
        }

        public override Task<BalancesReply> GetBalances(Empty request, ServerCallContext context)
        {
            var balances = MongoDBAccessor<Balance>.
                GetMongoCollection(_configuration.GetSection("MongoDB:DBName").Value,
                                    _configuration.GetSection("MongoDB:BalancesCollectionName").Value).
                Find(FilterDefinition<Balance>.Empty).ToEnumerable();
            TranslateEnumerableToObjects(balances, out BalancesReply reply);

            return Task.FromResult(reply);
        }

        private void TranslateEnumerableToObjects(IEnumerable<Balance> balances, out BalancesReply reply)
        {
            reply = new BalancesReply();

            foreach (var obj in balances)
            {
                var protoObj = new BalanceObject()
                {
                    Id = obj.Id.ToString(),
                    Storage = obj.StorageName,
                    Balance = (float)obj.Money
                };
                reply.Balances.Add(protoObj);
            }
        }
    }
}