using CalculationLib;

using Grpc.Core;

using Microsoft.Extensions.Configuration;

using MongoDB.Driver;

using Newtonsoft.Json;

using Serilog;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BCHGrpcService.Services
{
    public class BCHGrpcService : BCHGrpc.BCHGrpcBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BCHGrpcService(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override Task<RatingReply> GetRatingByPassport(RatingRequest request, ServerCallContext context)
        {
            IEnumerable<CreditHistory> clientCredits;

            try
            {
                clientCredits = MongoDBAccessor<Client>.
                                GetMongoCollection(_configuration.GetSection("MongoDB:DBName").Value,
                                                    _configuration.GetSection("MongoDB:CollectionName").Value).
                                Find(x => x.Passport == request.PassportNumber).
                                First().CreditHistory;
            }
            catch (InvalidOperationException e)
            {
                _logger.Information(e, $"Passport {request.PassportNumber} not found");

                throw new RpcException(new Status(StatusCode.NotFound, request.PassportNumber));
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);

                throw new RpcException(new Status(StatusCode.Internal, $"Internal error: {e.Message}"));
            }

            var response = new RatingReply
            {
                Rating = CreditCalculations.GetCreditRating(clientCredits)
            };

            return Task.FromResult(response);
        }

        public override Task<CreditHistoryReply> GetCreditHistory(CreditHistoryRequest request, ServerCallContext context)
        {
            object creditHistories;
            try
            {
                creditHistories = MongoDBAccessor<Client>.
                    GetMongoCollection(_configuration.GetSection("MongoDB:DBName").Value,
                                        _configuration.GetSection("MongoDB:CollectionName").Value).
                    Find(x => x.Passport == request.PassportNumber).
                    First().CreditHistory;
            }
            catch (InvalidOperationException e)
            {
                _logger.Debug(e, e.Message);
                throw new RpcException(new Status(StatusCode.NotFound, request.PassportNumber));
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);

                throw new RpcException(new Status(StatusCode.Internal, $"Internal error: {e.Message}"));
            }

            var reply = new CreditHistoryReply() { CreditHistoryJSON = JsonConvert.SerializeObject(creditHistories) };

            return Task.FromResult(reply);
        }
    }
}