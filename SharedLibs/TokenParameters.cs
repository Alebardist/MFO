namespace SharedLib
{
    public class TokenParameters
    {
        public string SecretKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }

        public TokenParameters(string secretKey, string validIssuer, string validAudience)
        {
            SecretKey = secretKey;
            ValidIssuer = validIssuer;
            ValidAudience = validAudience;
        }
    }
}