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

    public bool isRoaminghrs;
    private void Update()
    {
        m_gameTimer=GameDataDNDL.Instance.GetGameState==Constants.GameState.PlayGame;
        if (m_gameTimer)
        {
            _currentSec += Time.deltaTime;
            //Starting Ai Roaming At morning Hrs
            if (_currentSec >= (IRLMinsForInGameDay * ((6f / 24f) * 60f)) && !DayFlag)
            {
                isRoaminghrs = true;
                //Debug.Log(CustomLogs.CC_TagLog("Time Manager", $"Current Day Phase{m_phase}, {GetTime(true)}"));
            }
            if (_currentSec >= (IRLMinsForInGameDay * ((8f/24f)*60f)) && !DayFlag)
            {
                //Set active Timer for the kitchen open
                m_phase = DayPhase.KitchenOpen;
                DayFlag = true;
                HUDManagerDNDL.Instance.ShopDisable();//Change this to an Event
                //Debug.Log(CustomLogs.CC_TagLog("Time Manager",$"Current Day Phase{m_phase}, {GetTime(true)}"));
            }
            if (_currentSec >= (IRLMinsForInGameDay * ((18f / 24f) * 60f)) && !KitchenFlag)
            {
                //Set active Timer for the kitchen Close
                m_phase = DayPhase.KitchenClosed;
                KitchenFlag = true;
                HUDManagerDNDL.Instance.ShopEnable();//Change this to an Event
                //Debug.Log(CustomLogs.CC_TagLog("Time Manager", $"Current Day Phase{m_phase}, {GetTime(true)}"));
            }
            //Stoping Ai Roaming At night Hrs
            if (_currentSec >= (IRLMinsForInGameDay * ((22f / 24f) * 60f)) && !DayFlag)
            {
                isRoaminghrs = false;
                //Debug.Log(CustomLogs.CC_TagLog("Time Manager", $"Current Day Phase{m_phase}, {GetTime(true)}"));
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
                //Debug.Log(CustomLogs.CC_TagLog("Time Manager", $"Current Day Phase{_currentday}, {GetTime(true)}"));
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
    public float GetHrs(float time)
    {
        //var _sec = (IRLMinsForInGameDay * 60);
        var t = (time / 60) * m_TotalSecondPerDayIRL;
        var hr = t / 3600;
        hr = hr > 12 ? hr - 12 : hr;
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
    public float GetMins(float time)
    {
        //var _sec = (IRLMinsForInGameDay * 60);
        var t = (time / 60) * m_TotalSecondPerDayIRL;
        var hr = (int)t / 3600;
        var mins = ((int)(t) - (hr * 3600)) / 60;
        return mins;
    }
    public float GetSec(float time)
    {
        //var _sec = (IRLMinsForInGameDay * 60);
        var t = (time / 60) * m_TotalSecondPerDayIRL;
        var hr = (int)t / 3600;
        var mins = ((int)(t) - (hr * 3600)) / 60;
        return mins;
    }
    public string GetTimerinFormate(float timeinSec)
    {
        //var _sec = (IRLMinsForInGameDay * 60);
       ///* var t = (timeinSec / 60)/*/* * m_TotalSecondPerDayIRL*/;
        var hr = (int)timeinSec / 3600;
        var mins = ((int)(timeinSec) - (hr * 3600)) / 60;
        var sec= (int)timeinSec -(mins*60);
        //hr = is24hrs ? hr : hr > 12 ? hr - 12 : hr;
        //Debug.Log("Total Sec:" + t + " hrs: " + hr + " Mins:" + mins);
        return hr>0? $"{hr.ToString("00")}:{mins.ToString("00")}": $"{mins.ToString("00")}:{sec.ToString("00")}";
    } 
    public int GetTotalDays() => (int)_currentday;

}
