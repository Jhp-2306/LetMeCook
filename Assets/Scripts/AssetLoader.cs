using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Util;

public class AssetLoader : Singletonref<AssetLoader>
{
    public List<Sprite> sprites;

    public List<SO_IngredientData> ingredients;
    [SerializeField]
    private List<GameObject> EquipmentPrefab;
    public Sprite iconMissing;

    public GameObject itemPrefab;
    public GameObject PlatesPrefab;
    public Sprite GetSpriteFromList(int index) => sprites[index];

    public bool isAddressableLoaded;

    private Dictionary<string, AsyncOperationHandle<IList<GameObject>>> batchcache;
    private Dictionary<string, AsyncOperationHandle<GameObject>> cache;
    private Dictionary<string, AsyncOperationHandle<IList<Sprite>>> Spritescache;

    public bool GetisAddressableLoaded() => isAddressableLoaded;
    public void Addressableinit()
    {
        isAddressableLoaded = false;
        batchcache = new Dictionary<string, AsyncOperationHandle<IList<GameObject>>>();
        Spritescache = new Dictionary<string, AsyncOperationHandle<IList<Sprite>>>();
        LoadAssetBatchFromAddressable();
    }
    private void OnDestroy()
    {
        ReleaseAllAddressableCatch();
    }
    private void OnApplicationQuit()
    {
        ReleaseAllAddressableCatch();
    }

    public SO_IngredientData GetIngredientSO(IngredientType type, ProcessStatus process = ProcessStatus.None)
    {
        foreach (var ing in ingredients)
        {
            if (process == ing.process)
                if (ing.type == type) return ing;
        }
        return null;
    }
    public Sprite GetIngredientIcon(IngredientType type, ProcessStatus process = ProcessStatus.None)
    {
        foreach (var ing in ingredients)
        {
            if (process == ing.process)
                if (ing.type == type) return ing.icon;
        }
        return iconMissing;

    }

    public GameObject GetEquipmetPrefab(string _id)
    {
        foreach (var obj in EquipmentPrefab)
        {
            //Debug.Log(_id + " " + obj.name + " " + obj.name.Equals(_id));
            if (obj.name.Equals(_id)) return obj;
        }
        return null;
    }
    public Sprite GetIcons(string _id)
    {
        foreach (var obj in sprites)
        {
            Debug.Log(_id + " " + obj.name + " " + obj.name.Equals(_id));
            if (obj.name.Equals(_id)) return obj;
        }
        return null;
    }
    private void LoadAssetBatchFromAddressable()
    {
        Addressables.LoadAssetsAsync<GameObject>("Equipments")
              .Completed += (handle) =>
       {
           EquipmentPrefab = new List<GameObject>(handle.Result);
           Debug.Log(EquipmentPrefab.Count);
           batchcache.Add("Equipments", handle);
           isAddressableLoaded = true;
       };
        Addressables.LoadAssetsAsync<Sprite>("Icons")
               .Completed += (handle) =>
               {
                   sprites = new List<Sprite>(handle.Result);
                   Debug.Log(EquipmentPrefab.Count);
                   Spritescache.Add("Icons", handle);
                   //isAddressableLoaded = true;
               };
    }


    private void ReleaseAddressableCatch(string _id)
    {
        if (cache.ContainsKey(_id))
        {
            cache.Remove(_id, out var handle);
            Addressables.Release(handle);
        }
    }

    private void ReleaseAllAddressableCatch()
    {
        Debug.Log("Calling Release");
        if (batchcache != null)
        {
            foreach (var item in batchcache.Values)
            {
                //batchcache.Remove(item,out var batch);
                Addressables.Release(item);
            }
            batchcache.Clear();
        }
        if (cache != null)
        {
            foreach (var item in cache.Keys)
            {
                //cache.Remove(item, out var batch);
                Addressables.Release(item);
            }
            cache.Clear();
        }
        if (Spritescache != null)
        {
            foreach (var item in Spritescache.Keys)
            {
                //cache.Remove(item, out var batch);
                Addressables.Release(item);
            }
            Spritescache.Clear();
        }
    }
}
