using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Util;

public class ObjectPoolingManager : Singletonref<ObjectPoolingManager>
{
    public enum PrefabTags
    {
        Ingredients,
        Plate,
        none
    }
    [System.Serializable]
    public struct PrefabData
    {
        public PrefabTags id;
        public GameObject data;
    }
    Dictionary<PrefabTags, List<GameObject>> entries;
    public List<PrefabData> PrefabAssets;
    public GameObject Parent;
    public int PreSwampAmount = 20;
    public void AddOrCreate(PrefabTags _tag, GameObject _object)
    {
        if (entries.ContainsKey(_tag))
        {
            entries[_tag].Add(_object);
           // Debug.Log("ADD a new object" + _tag + _object.name);
        }
        else
        {
            entries.Add(_tag, new List<GameObject>() { _object });
            //Debug.Log("ADD a new Tag"+_tag+_object.name);
        }
        _object.transform.SetParent(this.transform);    
        _object.GetComponent<IObjectPoolingSettings>().OPSetting_Reset();
    }
    public GameObject GetaPool(PrefabTags _tag)
    {
        if (entries.ContainsKey(_tag))
        {
            //foreach (var obj in entries[_tag])
            //{
            //    Debug.Log($"{obj.name}");
            //}
            if (entries[_tag].Count > 0)
            {
                var t = entries[_tag][0];
                entries[_tag].RemoveAt(0);
                t.SetActive(true);
                return t;
            }
            else
            {
                var t = SpawnObject(_tag);
                t.SetActive(true);
                return t;
            }
           
        }
        return null;
    }

    GameObject SpawnObject(PrefabTags _tag)
    {
        foreach (var temp in PrefabAssets)
        {
            if (temp.id == _tag)
            {
                var obj = Instantiate(temp.data, this.gameObject.transform);
                obj.SetActive(false);
                return obj;
            }
        }
        return null;
    }
    public void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        entries = new Dictionary<PrefabTags, List<GameObject>>();
        foreach (var temp in PrefabAssets)
        {
            for (int i = 0; i < PreSwampAmount; i++)
                AddOrCreate(temp.id, SpawnObject(temp.id));
        }
    }
}

public interface IObjectPoolingSettings
{
    public void OPSetting_Reset();
    public GameObject GetGameObject();
    
}
