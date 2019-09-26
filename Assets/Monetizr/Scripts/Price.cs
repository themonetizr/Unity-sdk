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
                //00, 10, 20 decimals are formatted as 0 1 2, and that doesn't look right, so let's fix that
                //amountString = (value == "0.0") ? "0.00" : value;
                var halves = value.Split(new[] {',', '.'});
                halves[1] = halves[1].PadRight(2, '0');
                amountString = halves[0] + "." + halves[1];
                decimal.TryParse(value, out amount);
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

