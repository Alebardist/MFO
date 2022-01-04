using System.ComponentModel;

using SharedLib;

using Xunit;

namespace MFOTest.SharedLib
{
    public class SharedFuncsTests
    {
        [Fact]
        [DisplayName("CheckCredentials - correct")]
        public void CheckCredentialsCorrectnessMustReturnExpectedTrue()
        {
            var actual = SharedFuncs.CheckCredentialsCorrectness("admin", "adminPass");

            Assert.True(actual);
        }

        [Fact]
        [DisplayName("CheckCredentials - incorrect")]
        public void CheckCredentialsCorrectnessMustReturnExpectedFalse()
        {
            var actual = SharedFuncs.CheckCredentialsCorrectness("wrong name", "wrong pass");

            Assert.False(actual);
        }

        [Fact]
        [DisplayName("GenerateToken")]
        public void GenerateTokenMustreturnNotEmptyToken()
        {
            var tokenParams = new TokenParameters("some keykeykeykey", "Issuer", "Audience");

            var actual = SharedFuncs.GenerateToken("admin", "some role", tokenParams);

            Assert.NotEmpty(actual);
        }
    }
}