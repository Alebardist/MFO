using System;
using System.Collections.Generic;
using System.Linq;

using SharedLib.DTO;

namespace CalculationLib
{
    public static class CreditCalculations
    {
        public const int SCORE_FOR_PAYED_DEBTS = 20;
        public const int SCORE_FOR_OVERDUES = -5;
        public const int SCORE_FOR_HUGE_PAYED_DEBTS = 15;

        public static int GetCreditRating(IEnumerable<CreditHistory> creditHistories)
        {
            decimal creditRating = 0;

            var averageLoan = GetAverageDebt(creditHistories);

            creditRating += averageLoan / 100;
            creditRating += GetScoreForPayedDebts(creditHistories);
            creditRating += GetScoreForOverdues(creditHistories);

            return Convert.ToInt32(creditRating);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="creditHistories"></param>
        /// <returns></returns>
        private static decimal GetAverageDebt(IEnumerable<CreditHistory> creditHistories)
        {
            return creditHistories.Average(x => x.Summ);
        }

        /// <summary>
        /// Calculates score for payed debts
        /// </summary>
        /// <param name="creditHistories"></param>
        /// <returns></returns>
        private static int GetScoreForPayedDebts(IEnumerable<CreditHistory> creditHistories)
        {
            return creditHistories.Count(x => x.IsPayed) * SCORE_FOR_PAYED_DEBTS;
        }

        /// <summary>
        /// Calculates negative score for overdues
        /// </summary>
        /// <param name="creditHistories"></param>
        /// <returns></returns>
        private static int GetScoreForOverdues(IEnumerable<CreditHistory> creditHistories)
        {
            return creditHistories.Sum(x => x.Overdues) * SCORE_FOR_OVERDUES;
        }

        /// <summary>
        /// Counts payed debts which bigger than 20% of Month salary
        /// </summary>
        /// <param name="creditHistories"></param>
        /// <returns>rating scores</returns>
        /// <exception cref="NotImplementedException"></exception>
        private static int GetScoreForPayedHugeDebts(IEnumerable<CreditHistory> creditHistories)
        {
            throw new NotImplementedException();
        }
    }
}