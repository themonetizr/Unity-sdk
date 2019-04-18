using Assets.SingletonPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMono : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MonetizrClient.Instance.Init("4D2E54389EB489966658DDD83E2D1", "1794883780674");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
