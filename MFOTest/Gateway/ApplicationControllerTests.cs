namespace MFOTest.Gateway
{
    public class ApplicationControllerTests
    {
        private readonly HttpClient _httpClient = new();

        public ApplicationControllerTests()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:44317/api/Applications");
        }

        [Fact]
        public void CreateNewApplicationMustCreateDocumentInCollection()
        {
            CreditApplication creditApplication = new CreditApplication()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                PassportData = new Passport() { FIO = "123", SeriaAndNumber = "1234" },
                ContactPhoneNumber = "123",
                WishedAmount = 40000,
                TermInDays = 40,
                MonthIncome = 23000,
                MonthlyCreditServiceSum = 1,
                Education = "colledge",
                JobObject = new Job() { CuratorsWorkPhone = "123", LocalAdress = "addr", OrganizationName = "Company" }
            };

            var result = _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}/CreateNewApplication", creditApplication).Result;

            result.EnsureSuccessStatusCode();

            var collection = MongoDBAccessor<CreditApplication>.GetMongoCollection("CreditApplicationsService",
                                                                "CreditApplications");
            var idFromDB = collection.Find(x => x.Id == creditApplication.Id).First().Id;

            Assert.Equal(creditApplication.Id, idFromDB);

            collection.DeleteOne(x => x.Id == creditApplication.Id);
        }
    }
}
