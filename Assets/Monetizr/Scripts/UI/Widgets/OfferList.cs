using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr.UI.Widgets
{
	[DefaultExecutionOrder(4672)]
	public class OfferList : MonoBehaviour
	{
		public bool loadOnStart = true;
		public Transform listHolder;
		public GameObject template;
		private List<GameObject> currentItems = new List<GameObject>();
		private bool _loading = false;

		
		private void Start()
		{
			if(template != null)
				template.SetActive(false);
			
			if(loadOnStart)
				LoadOffers();
		}
		
		

		public void LoadOffers()
		{
			if (_loading) return;
			if (template == null)
			{
				Debug.LogError("<b>MONETIZR: </b>Offer List template is not set");
				return;
			}
			
			if (listHolder == null)
			{
				Debug.LogError("<b>MONETIZR: </b>Offer List list holder is not set");
				return;
			}

			_loading = true;
			MonetizrClient.Instance.AllProducts(UpdateList);
		}

		public void UpdateList(List<ListProduct> newProducts)
		{
			// Clear currently shown offers
			// Object pooling would be nice, but the tradeoff is not immense here
			currentItems.ForEach(Destroy);
			currentItems.Clear();

			if (newProducts == null)
			{
				Debug.LogWarning("<b>MONETIZR: </b>Offer list did not load any offers");
			}
			else
			{
				newProducts.ForEach(p =>
				{
					GameObject newItem = Instantiate(template, listHolder, false);
					var item = newItem.GetComponentInChildren<OfferListItem>();
					item.Initialize(p);
					newItem.SetActive(true);
					currentItems.Add(newItem);
				});
			}

			_loading = false;
		}
	}
}
