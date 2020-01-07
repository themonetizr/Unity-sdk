using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

		public RectTransform productPageHolder;
		public ProductPageScript ProductPage;
		public Animator ProductPageAnimator;
		public AlertPage AlertPage;
		public GameObject LoadingIndicator;
		public Image loadingIndicatorBackground;
		public CanvasGroup loadingIndicatorCanvas;
		public Animator LoadingIndicatorAnimator;
		public Animator nonFullscreenBackgroundAnimator;
		private static readonly int Opened = Animator.StringToHash("Opened");

		private float _currentScale = 0.7f;
		private bool _isBigScreen = false;
		public bool BigScreen { get { return _isBigScreen; } }

		public float CurrentScale
		{
			get { return _currentScale; }
		}

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

		public void SetProductPageScale(float scale)
		{
			productPageHolder.localScale = Vector3.one * scale;
			ProductPage.SetOutline(!Mathf.Approximately(scale, 1));
			_currentScale = scale;

			var c = loadingIndicatorBackground.color;
			c.a = Mathf.Approximately(scale, 1) ? 1f : 0f;
			loadingIndicatorBackground.color = c;
		}

		public void SetBigScreen(bool bs)
		{
			_isBigScreen = bs;
			ProductPage.SetOutline(bs);
			if (!bs) return;
			
			SetScale(_currentScale);

			var c = loadingIndicatorBackground.color;
			c.a = 0f;
			loadingIndicatorBackground.color = c;
		}

		public void SetScale(float s)
		{
			if (!_isBigScreen) return;
			productPageHolder.localScale = Vector3.one * s;
			_currentScale = s;
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
			if (loadingIndicatorCanvas.alpha > 0.1f) return true;
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
				foreach (var iView in ProductPage.ImageViewers)
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

				if (ProductPage.selectionManagerBigScreen.IsOpen())
				{
					ProductPage.selectionManagerBigScreen.HideSelection(true);
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

		public void SelectWhenInteractable(Selectable s)
		{
			if(s != null)
				StartCoroutine(_SelectWhenInteractable(s));
		}
		
		IEnumerator _SelectWhenInteractable(Selectable s)
		{
			while (!s.IsInteractable()) yield return null;
			yield return null;
			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(s.gameObject);
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
			
			if(!Mathf.Approximately(_currentScale, 1))
				nonFullscreenBackgroundAnimator.SetBool(Opened, AnyUIOpen());
		}
	}
}