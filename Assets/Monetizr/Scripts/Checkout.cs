using System;
using System.Collections.Generic;
using System.Linq;
using Monetizr.Dto;
using UnityEngine;

namespace Monetizr
{
	public class Checkout
	{
		public class Error
		{
			private string _message;
			private string _field;

			public string Message
			{
				get { return _message; }
			}

			public Error(string msg, string field)
			{
				_message = msg;
				_field = field;
			}
		}

		public class ShippingRate
		{
			private string _handle;
			private string _title;
			public Price Price;

			public ShippingRate(string handle, string title)
			{
				_handle = handle;
				_title = title;
			}

			public string Title
			{
				get { return _title; }
			}
		}

		public class Item
		{
			private string _title;
			private int _quantity;

			public Item(string title, int quantity)
			{
				_title = title;
				_quantity = quantity;
			}

			public string Title
			{
				get { return _title; }
			}

			public int Quantity
			{
				get { return _quantity; }
			}
		}

		public List<Error> Errors;
		
		private ShippingAddress _shippingAddress;
		public ShippingAddress ShippingAddress
		{
			get { return _shippingAddress; }
		}

		private string _id;
		public string Id
		{
			get { return _id; }
		}

		private string _webUrl;
		public string WebUrl
		{
			get { return _webUrl; }
		}

		public Price Subtotal;
		public Price Tax;
		public Price Total;
		public List<ShippingRate> ShippingOptions;
		public List<Item> Items;

		public Checkout()
		{
			Errors = new List<Error>();
			ShippingOptions = new List<ShippingRate>();
			Items = new List<Item>();
		}

		public void SetShippingAddress(ShippingAddress address)
		{
			_shippingAddress = address;
		}

		public static Checkout CreateFromDto(CheckoutProductResponse dto, ShippingAddress address)
		{
			var c = CreateFromDto(dto);
			c.SetShippingAddress(address);
			return c;
		}

		public static Checkout CreateFromDto(CheckoutProductResponse dto)
		{
			var c = new Checkout();
			var data = dto.data.checkoutCreate;

			if (data.checkoutUserErrors != null)
			{
				if (data.checkoutUserErrors.Count > 0)
				{
					data.checkoutUserErrors.ForEach(x =>
					{
						c.Errors.Add(new Error( x.message, x.field.Last()));
					});
					return c;
				}
			}

			// Exit early if we have errors.
			if (data.checkout == null) return c;
			var cDto = data.checkout;

			c._id = cDto.id;
			c._webUrl = cDto.webUrl;

			c.Subtotal.AmountString = cDto.subtotalPriceV2.amount;
			c.Subtotal.CurrencyCode = cDto.subtotalPriceV2.currencyCode;
			c.Subtotal.CurrencySymbol = cDto.subtotalPriceV2.currencyCode;
			
			c.Tax.AmountString = cDto.totalTaxV2.amount;
			c.Tax.CurrencyCode = cDto.totalTaxV2.currencyCode;
			c.Tax.CurrencySymbol = cDto.totalTaxV2.currencyCode;
			
			c.Total.AmountString = cDto.totalPriceV2.amount;
			c.Total.CurrencyCode = cDto.totalPriceV2.currencyCode;
			c.Total.CurrencySymbol = cDto.totalPriceV2.currencyCode;

			if (cDto.availableShippingRates != null)
			{
				cDto.availableShippingRates.shippingRates.ForEach(x =>
				{
					var rate = new ShippingRate(x.handle, x.title);
					rate.Price.AmountString = x.priceV2.amount;
					rate.Price.CurrencyCode = x.priceV2.currencyCode;
					rate.Price.CurrencySymbol = x.priceV2.currencyCode;
					
					c.ShippingOptions.Add(rate);
				});
			}
			
			cDto.lineItems.edges.ForEach(x =>
			{
				c.Items.Add(new Item(x.node.title, x.node.quantity));
			});

			return c;
		}
	}
}
