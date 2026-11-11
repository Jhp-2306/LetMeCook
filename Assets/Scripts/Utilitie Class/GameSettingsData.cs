using Constants;
using System;
using UnityEngine;

[Serializable]
public class GameSettingsData 
{
    private static GameSettingsData instance;
    private static readonly object _lock = new object();
    private const string file_name = "LMC_Setting_Data";
    public static GameSettingsData Instance
    {
        get
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new GameSettingsData();
                    }
                }
            }
            return instance;
        }
    }

    public GameSetting setting;
    public void init()
    {
        setting = new GameSetting();
        Debug.Log(CustomLogs.CC_TagLog($"-_-Game Setting-_-", "Init Done"));
    }
    public bool SaveInstance()
    {
        return SerializationManager.Save(file_name, Instance);
    }
    public bool LoadInstance()
    {
        try
        {
            var t = (GameSettingsData)SerializationManager.Load(file_name);
            setting = t.setting;
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(CustomLogs.CC_TagLog($"-_-Game Setting-_-", "NO USER DATA WAS FOUND"));
            return false;
        }
    }
}
[System.Serializable]
public class GameSetting
{
    public float MusicVolume=1;
    public float SFXVolume=1;
    public bool  AdsNotify=true;
    public bool  LocalNotify=true;
    public bool  PushNotify = true;
    public bool  TutorialDisable=false;

    public GameSetting() {
        MusicVolume = 1;
        SFXVolume = 1;
        AdsNotify = true;
        LocalNotify = true;
        PushNotify = true;
        TutorialDisable = false;
    }
    public GameSetting(GameSetting copydata)
    {
        MusicVolume = copydata.MusicVolume;
        SFXVolume = copydata.SFXVolume;
        AdsNotify = copydata.AdsNotify;
        LocalNotify = copydata.LocalNotify;
        PushNotify = copydata.PushNotify;
        TutorialDisable = copydata.TutorialDisable;
    }
}