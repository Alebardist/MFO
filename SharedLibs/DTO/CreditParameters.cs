namespace SharedLib.DTO
{
    public class CreditParameters
    {
        public int MoneyToLoan { get; set; }
        public int TermDays { get; set; }
        public int InterestRate { get; set; }

        public string PassportNumber { get; set; }
    }
}
