using System;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.Testing
{
	public class TestingSceneScript : MonoBehaviour
	{
		public Text inputText;
		public InputField tokenField;
		public InputField productField;

		private void Start()
		{
			Application.logMessageReceived += Application_logMessageReceived;
		}

		private void OnDestroy()
		{
			Application.logMessageReceived -= Application_logMessageReceived;
		}

		public void SetProduct(string p)
		{
			productField.text = p;
		}
		
		public void ShowButton()
		{
			MonetizrClient.Instance.SetAccessToken(tokenField.text);
			MonetizrClient.Instance.ShowProductForTag(productField.text);
		}

		private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
		{
			inputText.text += condition;
			inputText.text += "\n";

			if (inputText.text.Length > 2000)
			{
				inputText.text = inputText.text.Substring(inputText.text.Length - 1501, 1500);
			}
		}
	}
}
