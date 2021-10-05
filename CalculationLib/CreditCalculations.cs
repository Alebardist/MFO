using System;

using SharedLib.DTO;

namespace CalculationLib
{
    public class CreditCalculations
    {
        public int GetCreditRating(CreditHistory creditHistory, CreditParameters creditParameters)
        {
            var creditRating = 0f;

            var averageLoan = GetAverageLoan(creditHistory.Loans);

            creditRating = 100 / (creditParameters.MoneyToLoan / averageLoan);

            return Convert.ToInt32(creditRating);
        }

        private float GetAverageLoan(int[] loans)
        {
            int sumOfAllLoans = 0;

            for (int i = 0; i < loans.Length; i++)
            {
                sumOfAllLoans += loans[i];   
            }

            return sumOfAllLoans / loans.Length;
        }

    }
}
