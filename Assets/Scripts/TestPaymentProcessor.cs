using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monetizr;

public class TestPaymentProcessor : MonoBehaviour {
	private void Start()
	{
		MonetizrClient.Instance.MonetizrPaymentStarted += ProcessPayment;
	}

	public void ProcessPayment(Payment p)
	{
		StartCoroutine(_ProcessPayment(p));
	}

	private IEnumerator _ProcessPayment(Payment p)
	{
		yield return new WaitForSecondsRealtime(3f);
		p.Finish(Payment.PaymentResult.Successful, "Hooray, this works!");
	}
}
