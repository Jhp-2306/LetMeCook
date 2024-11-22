using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using UnityEngine.UI;
using Unity.VisualScripting;

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

    public UserDataLocal LocalData;
    public void Init()
    {
        LocalData = new UserDataLocal();
        Debug.Log("Init on SaveData");
    }
    public void NewGameSaveData()
    {
        LocalData = new UserDataLocal();
        SaveInstance();
    }
    public bool SaveInstance()
    {
        return SerializationManager.Save(file_name, Instance);
    }
    public void LoadInstance()
    {
        var t = (SaveData)SerializationManager.Load(file_name);
        Debug.Log($"check {t.LocalData == null},{LocalData == null}");
        LocalData = t.LocalData;
    }

}
[System.Serializable]
public class UserDataLocal
{
    public string Playername;
    public int PlayerLevel;
    public int Currency;
    public int Day;
    public int Time;

    public string StartTime;
    public string GameCloseTime;
    public string CurrentTime;

    public List<TableData> tableData;
    public UserDataLocal() {
        tableData = new List<TableData>();

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
[System.Serializable]
public class TableData
{
    public int Id;
    public string TableTop;
    public bool isTableEmpty;
    public string StartTime;
    public string EndTime;

    public TableData(int id, string tableTop, bool isTableEmpty, string startTime, string endTime)
    {
        Id = id;
        TableTop = tableTop;
        this.isTableEmpty = isTableEmpty;
        StartTime = startTime;
        EndTime = endTime;
    }
}