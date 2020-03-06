using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MonetizrDevTools : MonoBehaviour {
	[MenuItem("Monetizr/Edit Vertical")]
	static void PrepForVertical()
	{
		var pvCanvas = GameObject.Find("Product View").GetComponent<CanvasGroup>();
		pvCanvas.alpha = 1f;

		var portCanvas = GameObject.Find("Vertical Layout").GetComponent<CanvasGroup>();
		portCanvas.alpha = 1f;
		
		var horCanvas = GameObject.Find("Horizontal Layout").GetComponent<CanvasGroup>();
		horCanvas.alpha = 0f;
		
		var bsCanvas = GameObject.Find("Big Screen Layout").GetComponent<CanvasGroup>();
		bsCanvas.alpha = 0f;

		Selection.activeGameObject = portCanvas.gameObject;
	}
	
	[MenuItem("Monetizr/Edit Horizontal")]
	static void PrepForHorizontal()
	{
		var pvCanvas = GameObject.Find("Product View").GetComponent<CanvasGroup>();
		pvCanvas.alpha = 1f;

		var portCanvas = GameObject.Find("Vertical Layout").GetComponent<CanvasGroup>();
		portCanvas.alpha = 0f;
		
		var horCanvas = GameObject.Find("Horizontal Layout").GetComponent<CanvasGroup>();
		horCanvas.alpha = 1f;
		
		var bsCanvas = GameObject.Find("Big Screen Layout").GetComponent<CanvasGroup>();
		bsCanvas.alpha = 0f;
		
		Selection.activeGameObject = horCanvas.gameObject;
	}
	
	[MenuItem("Monetizr/Edit Big Screen")]
	static void PrepForBigScreen()
	{
		var pvCanvas = GameObject.Find("Product View").GetComponent<CanvasGroup>();
		pvCanvas.alpha = 1f;

		var portCanvas = GameObject.Find("Vertical Layout").GetComponent<CanvasGroup>();
		portCanvas.alpha = 0f;
		
		var horCanvas = GameObject.Find("Horizontal Layout").GetComponent<CanvasGroup>();
		horCanvas.alpha = 0f;
		
		var bsCanvas = GameObject.Find("Big Screen Layout").GetComponent<CanvasGroup>();
		bsCanvas.alpha = 1f;
		
		Selection.activeGameObject = bsCanvas.gameObject;
	}
	
	[MenuItem("Monetizr/Prep for play (need to apply prefab)")]
	static void StartPlay()
	{
		var pvCanvas = GameObject.Find("Product View").GetComponent<CanvasGroup>();
		pvCanvas.alpha = 0f;

		var portCanvas = GameObject.Find("Vertical Layout").GetComponent<CanvasGroup>();
		portCanvas.alpha = 0f;
		
		var horCanvas = GameObject.Find("Horizontal Layout").GetComponent<CanvasGroup>();
		horCanvas.alpha = 0f;
		
		var bsCanvas = GameObject.Find("Big Screen Layout").GetComponent<CanvasGroup>();
		bsCanvas.alpha = 0f;
	}
}
