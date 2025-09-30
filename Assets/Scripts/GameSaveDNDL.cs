using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Util;
using Constants;

public class GameSaveDNDL : Singletonref<GameSaveDNDL>
{
    public float SaveRateInSec;

    public static event Action DataUpdateBeforeSave = delegate { };
    

    //public SaveData mainGameData;
    private void Awake()
    {
        base.Awake();
        SaveData.Instance.Init();
        //StartCoroutine(SaveTick());
        Debug.Log($"check {SaveData.Instance.LocalData == null}");
    }

    IEnumerator SaveTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(SaveRateInSec);
            DataUpdateBeforeSave();
            SaveData.Instance.SaveInstance();
        }
    }
    public bool ForceSave()
    {
        SaveData.Instance.LocalData.Playername = "hallow";
        DataUpdateBeforeSave();
        return SaveData.Instance.SaveInstance();
    }
    public void NewGame()
    {
        SaveData.Instance.NewGameSaveData();
    }
    public void AddSaveData(SaveDataTemplate template)
    {
        SaveData.Instance.saveDataType.AddOrUpdate(template);
    }
    public static string GenerateId(string filler="")
    {
        return DateTime.Now.ToLongDateString()+"-"+DateTime.Now.ToLongTimeString()+"-"+filler+"{"+UnityEngine.Random.Range(0,int.MaxValue)+"}";
    }
}

