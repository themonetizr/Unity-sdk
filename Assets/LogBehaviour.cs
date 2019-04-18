using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBehaviour : MonoBehaviour
{

    public Text Text;
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        Text.text += logString+Environment.NewLine;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
