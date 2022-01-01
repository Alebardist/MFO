using System;
using System.Collections.Generic;

using CashboxGrpcService;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;
using Grpc.Net.Client;

using SharedLib.DTO;

using Xunit;

namespace MFOTests.CashboxTests
{
    public class CashboxTest
    {
        private GrpcChannel _channel;
        private Cashbox.CashboxClient _cashboxClient;

        public CashboxTest()
        {
            _channel = GrpcChannel.ForAddress("https://localhost:5001");
            _cashboxClient = new Cashbox.CashboxClient(_channel);
        }
                
        [Fact]
        public void LogInServiceMustReturnError()
        {
            var expected = LogInReply.Types.loginResult.WrongUserNameOrPassword;
            
            var request = new LogInRequest() { UserName = "admin", UserPassword = "wrong pass" };
            var actual = _cashboxClient.LogInService(request).Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LogInServiceMustReturnNotEmptyToken()
        {
            var request = new LogInRequest() { UserName = "admin", UserPassword = "adminPass" };
            var actual = _cashboxClient.LogInService(request).Token;

            Assert.NotEmpty(actual);
        }

        [Fact]
        public void SendMoneyReplyMustBeOK()
        {
            var expected = "";
            var request = new SendMoneyRequest()
            {
                CardNumber = "1234 1234 1234 1234"
            };

            var requestToLogin = new LogInRequest() { UserName = "admin", UserPassword = "adminPass" };
            var token = _cashboxClient.LogInService(requestToLogin).Token;

            Metadata headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");

            var actual = _cashboxClient.SendMoney(request, headers).ErrorMessage;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetBalancesMustReturnExpectedBalancesReply()
        {
            var requestToLogin = new LogInRequest() { UserName = "admin", UserPassword = "adminPass" };
            var token = _cashboxClient.LogInService(requestToLogin).Token;

            Metadata headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");

            var actual = _cashboxClient.GetBalances(new Empty(), headers);

            Assert.NotEmpty(actual.Balances);
        }
    }
}