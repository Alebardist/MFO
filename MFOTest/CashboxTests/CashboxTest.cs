using System.IdentityModel.Tokens.Jwt;
using System.Linq;

using CashboxGrpcService;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;
using Grpc.Net.Client;

using Xunit;

namespace MFOTests.CashboxTests
{
    public class CashboxTest
    {
        private readonly GrpcChannel _channel;
        private readonly Cashbox.CashboxClient _cashboxClient;

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
        public void LogInServiceMustReturnTokenWithExpectedRole()
        {
            var expected = "admin";

            var request = new LogInRequest() { UserName = "admin", UserPassword = "adminPass" };
            var token = _cashboxClient.LogInService(request).Token;
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var actual = jwtSecurityToken.Payload.Claims.ElementAt(2).Value;
            Assert.Equal(expected, actual);
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

            Metadata headers = new();
            headers.Add("Authorization", $"Bearer {token}");

            var actual = _cashboxClient.SendMoney(request, headers).ErrorMessage;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetBalancesMustReturnExpectedBalancesReply()
        {
            var requestToLogin = new LogInRequest() { UserName = "admin", UserPassword = "adminPass" };
            var token = _cashboxClient.LogInService(requestToLogin).Token;

            Metadata headers = new();
            headers.Add("Authorization", $"Bearer {token}");

            var actual = _cashboxClient.GetBalances(new Empty(), headers);

            Assert.NotEmpty(actual.Balances);
        }
    }
}