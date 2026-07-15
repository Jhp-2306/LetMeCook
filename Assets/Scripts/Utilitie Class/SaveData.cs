using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using UnityEngine.UI;
using Unity.VisualScripting;
using JetBrains.Annotations;

[Serializable]
public class SaveData
{
    private static SaveData instance;
    private static readonly object _lock = new object();
    private const string file_name = "LMC_Save_Data";
    public static SaveData Instance
    {
        get
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new SaveData();
                    }
                }
            }
            return instance;
        }
    }

    public PresistenUserData userData;
    public UserDataLocal LocalData;
    public SaveDataType<SaveDataTemplate> saveDataType;
    public void Init()
    {
        LocalData = new UserDataLocal();
        saveDataType = new SaveDataType<SaveDataTemplate>();

        Debug.Log("Init on SaveData");
    }
    public void NewGameSaveData()
    {
        LocalData = new UserDataLocal();
        saveDataType = new SaveDataType<SaveDataTemplate>();
        SaveInstance();
    }
    public bool SaveInstance()
    {
        return SerializationManager.Save(file_name, Instance);
    }
    public bool LoadInstance()
    {
        try
        {
            var t = (SaveData)SerializationManager.Load(file_name);
            Debug.Log($"check {t.LocalData == null},{LocalData == null}{t.saveDataType.GetAllTheData().Count}");
            LocalData = t.LocalData;
            saveDataType = t.saveDataType;
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(CustomLogs.CC_TagLog($"-_-Save Data-_-", "NO USER DATA WAS FOUND"));
            return false;
        }
    }



}
/// <summary>
/// this data will NOT get Deleted when loading a new save
/// </summary>
[System.Serializable]
public class PresistenUserData
{
    public string Playername;
    public int PlayerLevel;
    public PresistenUserData() { }
    public PresistenUserData(PresistenUserData copydata)
    {
        Playername = copydata.Playername;
        PlayerLevel = copydata.PlayerLevel;
    }
}
/// <summary>
/// this data will GET Deleted when loading a new save
/// </summary>
[System.Serializable]
public class UserDataLocal
{
    public int CurrencyAmount;
    public int Stars;
    public int Day;
    public int Time;
    public bool isNGDataSet;
    public string StartTime;
    public string GameCloseTime;
    public string CurrentTime;
    public Dictionary<perkSystem_Value,int> Perkdata;
    public List<IngredientType> ingredientsUnlocked;

    public UserDataLocal() {        
    ingredientsUnlocked = new List<IngredientType>();
    }

    public UserDataLocal(UserDataLocal copydata)
    {
        CurrencyAmount = copydata.CurrencyAmount;
        Day = copydata.Day;
        Time = copydata.Time;
        isNGDataSet = copydata.isNGDataSet;
        StartTime = copydata.StartTime;
        GameCloseTime = copydata.GameCloseTime;
        CurrentTime = copydata.CurrentTime;
        Stars = copydata.Stars;
        Perkdata=copydata.Perkdata;
        ingredientsUnlocked=copydata.ingredientsUnlocked;
    }
}

[System.Serializable]
public class InventoryDataLocal
{
    public Dictionary<IngredientType, int> IngredientData;
    public void AddIngredientData(IngredientType _type, int _qty)
    {
        if (IngredientData.ContainsKey(_type))
        {
            IngredientData[_type] += _qty;
        }
        else
        {
            IngredientData.Add(_type, _qty);
        }
    }
    public int GetIngredientData(IngredientType _type)
    {
        return IngredientData[_type];
    }
    public int GetIngredientData(int _id)
    {
        return IngredientData[(IngredientType)_id];
    }
}


