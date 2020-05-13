using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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
			if(inputText)
				Application.logMessageReceived += Application_logMessageReceived;
		}

		private void OnDestroy()
		{
			if(inputText)
				Application.logMessageReceived -= Application_logMessageReceived;
		}

		public void SwitchScene(string scene)
		{
			SceneManager.LoadScene(scene);
		}
		
		public void SetProduct(string p)
		{
			productField.text = p;
		}

		public void SetToken(string t)
		{
			tokenField.text = t;
		}
		
		public void ShowButton()
		{
			//TODO: MonetizrClient.Instance.SetAccessToken(tokenField.text);
			MonetizrClient.Instance.ShowProductForTag(productField.text);
		}
		
		public void ShowLockedButton()
		{
			//TODO: MonetizrClient.Instance.SetAccessToken(tokenField.text);
			MonetizrClient.Instance.ShowProductForTag(productField.text, true);
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
