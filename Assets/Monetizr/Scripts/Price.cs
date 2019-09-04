using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr
{
    public class Price
    {
        public string CurrencySymbol;
        public string CurrencyCode;
        private string amountString;
        public string AmountString
        {
            get
            {
                return amountString;
            }

            set
            {
                amountString = value;
                decimal.TryParse(amountString, out Amount);
            }
        }
        public decimal Amount;

        public string FormattedPrice
        {
            get
            {
                return CurrencySymbol + AmountString;
            }
        }
    }
}

