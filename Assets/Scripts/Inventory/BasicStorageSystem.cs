using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicStorageSystem<T> where T : IStorageItem
{
    struct BasicStorageSystemDataStruct
    {
        public List<string> items;        
        //public int size;
    }
    List<T> items;
    int size;
    BasicStorageSystemDataStruct data;
    public BasicStorageSystem(int _size)
    {
        items = new List<T>();
        size = _size;
    }

    public bool AddItems(T obj)
    {
        if (items.Count < size)
        {
            items.Add(obj);
            return true;
        }
        else
        {
            CustomLogs.CC_Log("Failed to add the things --- storage full", "red");
            return false;
        }
    }
    public void updateItem(T obj, int idx = -1)
    {
        if (idx < 0) return;
        items[idx] = obj;
    }
    public void RemoveItemAt(int idx)
    {
        if (idx < 0 || idx > items.Count) return;
        items.RemoveAt(idx);
    }
    public T GetItemAtIndex(int idx)
    {
        return items[idx];
    }
    public int GetIndexOfItem(T obj)
    {
        return items.IndexOf(obj);
        //return -1;
    }
    public List<T> GetAllItems() => items;

    public string GetAllDataInString()
    {
        List<string> list = new List<string>();
        foreach (var item in items)
        {
            list.Add(item.GetData());
        }
        data.items = list;
        var tempString = JsonUtility.ToJson(data);
        return tempString;
    }
    public static List<string> LoadDataFromString(string data)
    {
        var tempData = JsonUtility.FromJson<BasicStorageSystemDataStruct>(data);
        return tempData.items;
    }
    //public static int LoadSizeFromString(string data)
    //{
    //    var tempData = JsonUtility.FromJson<BasicStorageSystemDataStruct>(data);
    //    return size;
    //}
}

public interface IStorageItem
{
    public IngredientType Type { get; }
    public void RemoveQuanitity();
    public void AddQuanitity(int quanitity, out int remaining);
    public bool isMaxed();
    public int GetQuanitity();
    public string GetData();
    //public void LoadData(string data);
}
