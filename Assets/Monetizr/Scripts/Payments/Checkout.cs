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
			#pragma warning disable
			private string _field;
			#pragma warning enable

			public string Message { get; private set; }

			public Error(string msg, string field)
			{
				Message = msg;
				_field = field;
			}
		}

		public class ShippingRate
		{
			public Price Price;

			public ShippingRate(string handle, string title)
			{
				Handle = handle;
				Title = title;
				Price = new Price();
			}

			public string Title { get; private set; }
			public string Handle { get; private set; }
		}

		public class Item
		{
			public Item(string title, int quantity)
			{
				Title = title;
				Quantity = quantity;
			}

			public string Title { get; private set; }

			public int Quantity { get; private set; }
		}

		public List<Error> Errors;

		public ShippingAddress ShippingAddress { get; private set; }
		public string RecipientEmail { get; private set; }
		public Product Product { get; private set; }
		public Product.Variant Variant { get; private set; }

		public string Id { get; private set; }

		public string WebUrl { get; private set; }

		public Price Subtotal;
		public Price Tax;
		public Price Total;
		public List<ShippingRate> ShippingOptions;
		public ShippingRate SelectedShippingRate { get; private set; }
		public List<Item> Items;

		public Checkout()
		{
			Errors = new List<Error>();
			ShippingOptions = new List<ShippingRate>();
			Items = new List<Item>();
		}

		public void SetShippingAddress(ShippingAddress address)
		{
			ShippingAddress = address;
		}

		public void SetProduct(Product product)
		{
			Product = product;
		}
		public void SetVariant(Product.Variant variant)
		{
			Variant = variant;
		}

		public void SetShippingLine(ShippingRate rate)
		{
			SelectedShippingRate = rate;
		}

		public void SetEmail(string email)
		{
			RecipientEmail = email;
		}

		public static Checkout CreateFromDto(CheckoutProductResponse dto, ShippingAddress address, Product.Variant variant)
		{
			var c = CreateFromDto(dto);
			c.SetShippingAddress(address);
			c.SetVariant(variant);
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

			c.Id = cDto.id;
			c.WebUrl = cDto.webUrl;

			c.Subtotal = new Price();
			c.Subtotal.AmountString = cDto.subtotalPriceV2.amount;
			c.Subtotal.CurrencyCode = cDto.subtotalPriceV2.currencyCode;
			c.Subtotal.CurrencySymbol = cDto.subtotalPriceV2.currencyCode;
			
			c.Tax = new Price();
			c.Tax.AmountString = cDto.totalTaxV2.amount;
			c.Tax.CurrencyCode = cDto.totalTaxV2.currencyCode;
			c.Tax.CurrencySymbol = cDto.totalTaxV2.currencyCode;
			
			c.Total = new Price();
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
		
		public void UpdateCheckout(ShippingAddress billing, Action<bool> checkoutUpdated)
		{
			var request = new Dto.CheckoutUpdatePostData();
			request.product_handle = Product.Tag;
			request.checkoutId = Id;
			request.email = RecipientEmail;
			request.shippingRateHandle = SelectedShippingRate.Handle;
			request.shippingAddress = ShippingAddress;
			request.billingAddress = billing ?? ShippingAddress;
            
			MonetizrClient.Instance.PostObjectWithResponse<Dto.UpdateCheckoutResponse>
			("products/updatecheckout", request, response =>
			{
				Errors.Clear();
				if (response == null)
				{
					Errors.Add(new Error( "Internal error occured", ""));
					checkoutUpdated(false);
					return;
				}
				var data = response.data;
				if (data.updateShippingAddress.checkoutUserErrors != null)
				{
					if (data.updateShippingAddress.checkoutUserErrors.Count > 0)
					{
						data.updateShippingAddress.checkoutUserErrors.ForEach(x =>
						{
							Errors.Add(new Error( x.message, x.field.Last()));
						});
						checkoutUpdated(false);
						return;
					}
				}
				
				if (data.updateShippingLine.checkoutUserErrors != null)
				{
					if (data.updateShippingLine.checkoutUserErrors.Count > 0)
					{
						data.updateShippingLine.checkoutUserErrors.ForEach(x =>
						{
							Errors.Add(new Error( x.message, x.field.Last()));
						});
						checkoutUpdated(false);
						return;
					}
				}
				
				var cDto = response.data.updateShippingLine.checkout;
				Subtotal = new Price
				{
					AmountString = cDto.subtotalPriceV2.amount,
					CurrencyCode = cDto.subtotalPriceV2.currencyCode,
					CurrencySymbol = cDto.subtotalPriceV2.currencyCode
				};

				Tax = new Price
				{
					AmountString = cDto.totalTaxV2.amount,
					CurrencyCode = cDto.totalTaxV2.currencyCode,
					CurrencySymbol = cDto.totalTaxV2.currencyCode
				};

				Total = new Price
				{
					AmountString = cDto.totalPriceV2.amount,
					CurrencyCode = cDto.totalPriceV2.currencyCode,
					CurrencySymbol = cDto.totalPriceV2.currencyCode
				};

				var ship = new ShippingRate(cDto.shippingLine.handle, cDto.shippingLine.title)
				{
					Price =
					{
						AmountString = cDto.shippingLine.priceV2.amount,
						CurrencyCode = cDto.shippingLine.priceV2.currencyCode,
						CurrencySymbol = cDto.shippingLine.priceV2.currencyCode
					}
				};
				SelectedShippingRate = ship;
				checkoutUpdated(true);
			});
		}
	}
}
