using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLogs 
{
    public static void CC_Log(string msg,string color)
    {
        Debug.Log($"<color={color}>{msg}</color>");
    }
    public static void CC_EventLog(string _eventName,string color,string msg,string msgColor)
    {
        Debug.Log($"<color={color}>{_eventName}</color><color={msgColor}> {msg}</color>");
    }
}
