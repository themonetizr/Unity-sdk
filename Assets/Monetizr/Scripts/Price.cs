using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        private string _amountString;

        /// <summary>
        /// Gets/sets the price as a <see cref="string"/> value.
        /// </summary>
        public string AmountString
        {
            get { return _amountString; }

            set
            {
                //00, 10, 20 decimals are formatted as 0 1 2, and that doesn't look right, so let's fix that
                //amountString = (value == "0.0") ? "0.00" : value;
                var halves = value.Split(new[] {',', '.'});
                if (halves.Length > 1)
                {
                    halves[1] = halves[1].PadRight(2, '0');
                    _amountString = halves[0] + "." + halves[1];
                }
                else
                {
                    //There are also currencies without any decimal prices
                    _amountString = halves[0];
                }

                decimal.TryParse(value, NumberStyles.Any,  CultureInfo.InvariantCulture, out _amount);
            }
        }

        private decimal _amount;
        /// <summary>
        /// Gets the price as a <see cref="decimal"/> value.
        /// </summary>
        public decimal Amount
        {
            get
            {
                return _amount;
            }
        }

        private decimal _originalAmount;
        /// <summary>
        /// Gets the non-discounted price as a <see cref="decimal"/> value. If there is no discount,
        /// returns 0.
        /// </summary>
        public decimal OriginalAmount
        {
            get
            {
                if (String.IsNullOrEmpty(_originalAmountString))
                {
                    return 0;
                }
                return _originalAmount;
            }
        }

        private string _originalAmountString = String.Empty;
        
        /// <summary>
        /// Gets/sets the non-discounted price as a <see cref="string"/> value.
        /// </summary>
        public string OriginalAmountString
        {
            get { return _originalAmountString; }
            set //Copy pasted from AmountString
            {
                //00, 10, 20 decimals are formatted as 0 1 2, and that doesn't look right, so let's fix that
                //amountString = (value == "0.0") ? "0.00" : value;
                var halves = value.Split(new[] {',', '.'});
                if (halves.Length > 1)
                {
                    halves[1] = halves[1].PadRight(2, '0');
                    _originalAmountString = halves[0] + "." + halves[1];
                }
                else
                {
                    //There are also currencies without any decimal prices
                    _originalAmountString = halves[0];
                }
                decimal.TryParse(value, out _originalAmount);
            }
        }

        /// <summary>
        /// Gets the price for display, with the currency symbol on the left.
        /// </summary>
        public string FormattedPrice
        {
            get
            {
                if (Amount == 0)
                {
                    return "Free";
                }
                if (char.IsLetter(CurrencySymbol[0]))
                {
                    return AmountString + " " + CurrencySymbol;
                }
                else
                {
                    return CurrencySymbol + AmountString;
                }
            }
        }
        
        /// <summary>
        /// Gets the non-discounted price for display, with the currency symbol on the left.
        /// </summary>
        public string FormattedOriginalPrice
        {
            get
            {
                if (Amount == 0)
                {
                    return "Free";
                }
                if (char.IsLetter(CurrencySymbol[0]))
                {
                    return OriginalAmountString + " " + CurrencySymbol;
                }
                else
                {
                    return CurrencySymbol + OriginalAmountString;
                }
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if this Price has a discount applied.
        /// </summary>
        public bool Discounted
        {
            get { return !String.IsNullOrEmpty(_originalAmountString); }
        }
    }
}

