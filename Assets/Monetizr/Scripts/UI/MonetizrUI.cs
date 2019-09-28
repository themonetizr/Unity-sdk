using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monetizr.UI
{
	public class MonetizrUI : MonoBehaviour
	{
		public delegate void MonetizrScreenOrientationDelegate(bool portrait);
		public MonetizrScreenOrientationDelegate ScreenOrientationChanged;

		public delegate void MonetizrScreenResolutionDelegate();
		public MonetizrScreenResolutionDelegate ScreenResolutionChanged;
		
		private bool _lastOrientation;
		private Vector2 _lastResolution;

		public ProductPageScript ProductPage;
		public Animator ProductPageAnimator;
		public AlertPage AlertPage;
		public GameObject LoadingIndicator;
		public Animator LoadingIndicatorAnimator;
		private static readonly int Opened = Animator.StringToHash("Opened");

		private void Start()
		{
			_lastOrientation = Utility.UIUtility.IsPortrait();
			_lastResolution = new Vector2(Screen.width, Screen.height);
		}

		public void SetProductPage(bool active)
		{
			//ProductPage.gameObject.SetActive(active);
			ProductPageAnimator.SetBool(Opened, active);
		}

		public void SetLoadingIndicator(bool active)
		{
			//LoadingIndicator.SetActive(active);
			if(MonetizrClient.Instance.LoadingScreenEnabled())
				
				LoadingIndicatorAnimator.SetBool(Opened, active);
			else
				LoadingIndicatorAnimator.SetBool(Opened, false);
		}

		/// <summary>
		/// Function for application developers to know whether Monetizr UI expects
		/// back button actions. Otherwise simultaneously active UIs can both read the back button.
		/// </summary>
		/// <returns>If back button is supposed to do something for Monetizr UI</returns>
		public bool BackButtonHasAction()
		{
			if(WebViewController.IsOpen())
			{
				return true;
			}
			if (AlertPage.IsOpen())
			{
				return true;
			}
			if (ProductPage.IsOpen())
			{
				return true;
			}
			return false;
		}

		public bool AnyUIOpen()
		{
			if (WebViewController.IsOpen()) return true;
			if (AlertPage.IsOpen()) return true;
			if (ProductPage.IsOpen()) return true;
			if (LoadingIndicatorAnimator.GetBool(Opened)) return true;
			return false;
		}

		public void HandleBackButton(bool fromSwipe = false)
		{
			if (WebViewController.IsOpen())
			{
				//WebViewController handles back button internally, so we ignore the rest.
				return;
			}
			if (AlertPage.IsOpen())
			{
				AlertPage.HideAlert();
				return;
			}
			if(ProductPage.IsOpen())
			{
				foreach (var iView in ProductPage.imageViewers)
				{
					if (iView.IsOpen() && !iView.IsPermanent())
					{
						iView.HideViewer();
						return;
					}
				}
				if(ProductPage.SelectionManager.IsOpen())
				{
					ProductPage.SelectionManager.HideSelection();
					return;
				}
				if (fromSwipe) return;
				ProductPage.CloseProductPage();
			}
		}

		public void HandleRightSwipe()
		{
			if (ProductPage.IsOpen())
			{
				if (ProductPage.SelectionManager.IsOpen())
				{
					ProductPage.SelectionManager.AnimateToPrevious();
				}
			}
		}

		public void HandleLeftSwipe()
		{
			if (ProductPage.IsOpen())
			{
				if (ProductPage.SelectionManager.IsOpen())
				{
					ProductPage.SelectionManager.AnimateToNext();
				}
			}
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				HandleBackButton();
			}

			bool thisOrientation = Utility.UIUtility.IsPortrait();
			if(_lastOrientation != thisOrientation)
			{
				//Send a trigger to update layouts on screen orientation change.
				//ProductPageScript definitely subscribes to this.
				if (ScreenOrientationChanged != null)
					ScreenOrientationChanged(thisOrientation);
			}
			_lastOrientation = thisOrientation;

			Vector2 thisResolution = new Vector2(Screen.width, Screen.height);
			if (thisResolution != _lastResolution)
			{
				if (ScreenResolutionChanged != null)
					ScreenResolutionChanged();
			}

			_lastResolution = thisResolution;
		}
	}
}