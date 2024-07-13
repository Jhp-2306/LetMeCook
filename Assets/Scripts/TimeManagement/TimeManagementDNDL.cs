using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class TimeManagementDNDL : Singletonref<TimeManagementDNDL>
{
    enum DayPhase
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
    void GameStartDay()
    {
        //if (SaveData.Instance.LocalData.StartTime != null)
        //{

        //}
    }
    private void Update()
    {
        if (m_gameTimer)
        {
            _currentSec += Time.deltaTime;

            if (_currentSec >= IRLMinsForInGameDay * 21 && !DayFlag)
            {
                //Set active Timer for the kitchen (Shop open)
                DayFlag = true;
                //Debug.Log(GetTimein24Hrs());
            }
            if (_currentSec >= IRLMinsForInGameDay*60)
            {
                _currentSec = 0;
                DayFlag = false;
                _currentday++;
                //Day completed (Shop close)
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

}
