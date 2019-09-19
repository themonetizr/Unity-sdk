using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr
{
    /// <summary>
    /// Contains information and methods about a product variant pricing.
    /// Use <see cref=">FormattedPrice"/> to get a price for display.
    /// Use <see cref="AmountString"/> to set a price.
    /// Use <see cref="Amount"/> only to GET the price as a <see cref="decimal"/> value.
    /// </summary>
    public class Price
    {
        public string CurrencySymbol;
        public string CurrencyCode;
        private string amountString;
        /// <summary>
        /// Gets/sets the price as a <see cref="string"/> value.
        /// </summary>
        public string AmountString
        {
            get
            {
                return amountString;
            }

            set
            {
                amountString = (value == "0.0") ? "0.00" : value;
                decimal.TryParse(amountString, out amount);
            }
        }
        private decimal amount;
        /// <summary>
        /// Gets the price as a <see cref="decimal"/> value.
        /// </summary>
        public decimal Amount
        {
            get
            {
                return amount;
            }
        }

        /// <summary>
        /// Gets the price for display, with the currency symbol on the left.
        /// </summary>
        public string FormattedPrice
        {
            get
            {
                return CurrencySymbol + AmountString;
            }
        }
    }
}

