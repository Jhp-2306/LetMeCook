using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLogs 
{
    public static string CC_Log(string msg,string color)
    {
       return $"<color={color}>{msg}</color>";
    }
    public static string CC_EventLog(string _eventName,string color,string msg,string msgColor)
    {
        return $"<color={color}>{_eventName}</color><color={msgColor}> {msg}</color>";
    }
    public static string CC_TagLog(string tag, string msg)
    {

        return $"<color=red>***{tag}***\t</color>: {msg}";
    }

}
