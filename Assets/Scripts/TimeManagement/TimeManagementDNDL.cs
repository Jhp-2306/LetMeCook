using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class TimeManagementDNDL : Singletonref<TimeManagementDNDL>
{
    public enum DayPhase
    {
        Preparation,
        KitchenOpen,
        KitchenClosed
    }
    [SerializeField]
    private bool m_gameTimer;


    [SerializeField] float IRLMinsForInGameDay = 10f;
    const float m_TotalSecondPerDayIRL = 86400;   
    private float _currentSec;
    private float _currentday;
    private bool DayFlag;
    private bool KitchenFlag;
    private DayPhase m_phase;
    public DayPhase CurrentDayPhase { get => m_phase; }
  
    private void Update()
    {
        if (m_gameTimer)
        {
            _currentSec += Time.deltaTime;
            if (_currentSec >= (IRLMinsForInGameDay * ((8f/24f)*60f)) && !DayFlag)
            {
                //Set active Timer for the kitchen open
                m_phase = DayPhase.KitchenOpen;
                DayFlag = true;
                HUDManagerDNDL.Instance.ShopDisable();//Change this to an Event
                Debug.Log(CustomLogs.CC_TagLog("Time Manager",$"Current Day Phase{m_phase}, {GetTime(true)}"));
            }
            if (_currentSec >= (IRLMinsForInGameDay * ((18f / 24f) * 60f)) && !KitchenFlag)
            {
                //Set active Timer for the kitchen Close
                m_phase = DayPhase.KitchenClosed;
                KitchenFlag = true;
                HUDManagerDNDL.Instance.ShopEnable();//Change this to an Event
                Debug.Log(CustomLogs.CC_TagLog("Time Manager", $"Current Day Phase{m_phase}, {GetTime(true)}"));
            }
            if (_currentSec >= IRLMinsForInGameDay*60)
            {
                //DateTime.UtcNow.DayOfWeek
                //Day completed 
                m_phase = DayPhase.Preparation;
                _currentSec = 0;
                DayFlag = false;
                KitchenFlag = false;
                _currentday++;
                HUDManagerDNDL.Instance.ShopEnable();//Change this to an Event
                Debug.Log(CustomLogs.CC_TagLog("Time Manager", $"Current Day Phase{_currentday}, {GetTime(true)}"));
            }
        }

    }
    public string GetTimein24Hrs()
    {
        var _sec = (IRLMinsForInGameDay * 60);
        var t = (_currentSec /_sec ) * m_TotalSecondPerDayIRL;
        var hr = (int)t / 3600;
        var mins = ((int)(t)-(hr*3600))/60;
        //Debug.Log("Total Sec:" + t + " hrs: " + hr + " Mins:" + mins);
        return $"{hr.ToString("00")}:{mins.ToString("00")}";
    }
    public string GetTime(bool is24hrs)
    {
        var _sec = (IRLMinsForInGameDay * 60);
        var t = (_currentSec / _sec) * m_TotalSecondPerDayIRL;
        var hr = (int)t / 3600;
        var mins = ((int)(t) - (hr * 3600)) / 60;
        hr = is24hrs ? hr : hr > 12 ? hr - 12 : hr;
        //Debug.Log("Total Sec:" + t + " hrs: " + hr + " Mins:" + mins);
        return $"{hr.ToString("00")}:{mins.ToString("00")}";
    }
    public float GetHrs()
    {
        var _sec = (IRLMinsForInGameDay * 60);
        var t = (_currentSec / _sec) * m_TotalSecondPerDayIRL;
        var hr = t / 3600;
        hr =  hr > 12 ? hr - 12 : hr;
        return hr;
    }
    public float GetMins()
    {
        var _sec = (IRLMinsForInGameDay * 60);
        var t = (_currentSec / _sec) * m_TotalSecondPerDayIRL;
        var hr = (int)t / 3600;
        var mins = ((int)(t) - (hr * 3600)) / 60;
        return mins;
    }

}
