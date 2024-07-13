using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
public class SaveData
{
    private static SaveData instance;
    public static SaveData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SaveData();
            }
            return instance;
        }
    }

    public UserDataLocal LocalData;
    
    public static void SaveInstance()
    {
        SerializationManager.Save("LMC_Save_Data", instance);
    }
    public static void LoadInstance()
    {
        instance =(SaveData)SerializationManager.Load("LMC_Save_Data");
    }

}



public class IngredientData
{
    public IngredientType Type;
    public int Count;
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
    
    public void setGameStartTime(DateTime _dateTime)
    {
        StartTime = _dateTime.ToString(Constants.Constant.DateTimeFormate);
    }
    public void setGameCloseTime(DateTime _dateTime)
    {
        GameCloseTime= _dateTime.ToString(Constants.Constant.DateTimeFormate);
    }
}


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
