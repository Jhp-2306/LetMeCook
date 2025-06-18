using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicStorageSystem<T> where T : IStorageItem
{
    List<T> items;
    int size;
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
        if(idx < 0||idx>items.Count) return;
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
    public List<T> GetAllItems()=>items;
}

public interface IStorageItem
{
    public IngredientType Type { get; }
    public void RemoveQuanitity();
    public void AddQuanitity(int quanitity, out int remaining);
    public bool isMaxed();
    public int GetQuanitity();
}
