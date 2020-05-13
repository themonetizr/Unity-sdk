using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Monetizr;
using UnityEngine;
using UnityEditor;

namespace Monetizr.Editor
{
	public class MonetizrEditor {
		private static string GetAssetPath(string name)
		{
			string[] res = Directory.GetFiles(Application.dataPath, name, SearchOption.AllDirectories);
			if (res.Length == 0)
			{
				return null;
			}

			var path = res[0].Replace(Application.dataPath, "Assets").Replace("\\", "/");
			return path;
		}
		
		public static void CreateMonetizrSettings()
		{
			var settings = ScriptableObject.CreateInstance<MonetizrSettings>();
			try
			{
				settings.SetPrefabs(
					AssetDatabase.LoadAssetAtPath<GameObject>(GetAssetPath("__Monetizr UI Prefab.prefab")),
					AssetDatabase.LoadAssetAtPath<GameObject>(GetAssetPath("__Monetizr Web View Prefab.prefab"))
				);
			}
			catch
			{
				EditorUtility.DisplayDialog("Error in Monetizr setup", "Could not find the required prefabs.", "OK"); 
				return;
			}

			if (!AssetDatabase.IsValidFolder("Assets/Resources"))
			{
				AssetDatabase.CreateFolder("Assets", "Resources");
			}
			
			AssetDatabase.CreateAsset(settings, "Assets/Resources/MonetizrSettings.asset");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static MonetizrSettings GetMonetizrSettings()
		{
			return Resources.Load<MonetizrSettings>("MonetizrSettings");
		}
	}
}
