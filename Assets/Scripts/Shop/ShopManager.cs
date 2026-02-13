using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Util;

public class ShopManager : Singletonref<ShopManager>
{
    [SerializeField]
    SO_EquipmentData SO_EquipmentData;
    //public SO_EquipmentDataList ItemList;
    public InteractiveBlock CurrentSelectTable;

    public InteractionBTN interactionBTN;
    float rotateduration=025f;
    //public Table GetCurrentTable { get => CurrentSelectTable; }
    public class ShopItems
    {
        EquipmentType type;
        string name;
        int price;
        string icon;
        string PrefabID;

        public ShopItems(string name, int price, EquipmentType _type, string _prefab, string icon = null)
        {
            this.name = name;
            this.price = price;
            this.icon = icon;
            this.type = _type;
            this.PrefabID = _prefab;
        }
        public string GetName() => name;
        public int GetPrice() => price;
        public string GetIcon() => icon;
        public string GetPrefab() => PrefabID;
        public EquipmentType GetEquipmentType() => type;
    }
    public List<ShopItems> Items;
    public int MaxShopItems;
    public ShopUI _ShopUI;
    public List<ShopItems> GetItems() => Items;
    public PlacementHUD buildingHud;
    GameObject CurrentPurchaseObject;
    bool isBuildMode;
    public GameObject GetCurrentPurchaseObject { get => CurrentPurchaseObject; }
    private string _currentPrefabID;
    private void Start()
    {
        Items = new List<ShopItems>();
        if (GameDataDNDL.Instance.ItemList != null)
        {
            foreach (var item in GameDataDNDL.Instance.ItemList.equipmentDataList)
            {
                //Debug.Log(item.Prefab==null);
                var t = new ShopItems(item.name, item.price, item.Type, item.Prefab,item.Icon);
                Items.Add(t);
            }
            MaxShopItems = Items.Count;
        }
        else
        {
            Debug.LogError("No items in the shop");
        }
        _ShopUI.init();
    }




    public void CloseShop()
    {
        HUDManagerDNDL.Instance.CloseShop();
    }

    public void BuildModeActivate(string _prefab)
    {
        var templistpos = GameDataDNDL.Instance.GetGrid().GetAvailableNeighbourPosition(GameDataDNDL.Instance.GetPlayer().gameObject.transform.position);
        if (templistpos.Count > 0)
        {
            var position = templistpos[UnityEngine.Random.RandomRange(0, templistpos.Count)];
            var go = Instantiate(AssetLoader.Instance.GetEquipmetPrefab(_prefab), position, Quaternion.identity);
            _currentPrefabID = _prefab;
            CurrentPurchaseObject = go;
            CurrentPurchaseObject.GetComponent<Collider>().isTrigger = true;
            buildingHud.gameObject.SetActive(true);
            buildingHud.gameObject.transform.SetParent(go.transform);
            buildingHud.gameObject.transform.localPosition = Vector3.up * 1.75f;
            //Enter Building mode
            InputManager.Instance.ChangeMode(CameraTargetMode.BuildingMode, go);
            InputManager.Instance.EnterBuildingMode();
            isBuildMode = true;
            StartCoroutine(CheckConfirmBTN());
        }
    }

    public void CancelPurchase()
    {
        if (CurrentPurchaseObject != null)
        {
            buildingHud.gameObject.transform.SetParent(this.gameObject.transform);
            buildingHud.gameObject.SetActive(false);
            Destroy(CurrentPurchaseObject);
            //Cancel purchases

            //Exit Building mode
            InputManager.Instance.ChangeMode(CameraTargetMode.PlayerCam, GameDataDNDL.Instance.GetPlayer().gameObject);
            InputManager.Instance.ExitBuildingMode();
            isBuildMode= false;
        }

    }
    public void RotateObject()
    {
        if (CurrentPurchaseObject != null)
        {
            CurrentPurchaseObject.transform.eulerAngles += Vector3.up * 90;
            //StartCoroutine(RotateObjectLoop());
        }
    }
    IEnumerator RotateObjectLoop()
    {
        float elapsed = 0f;
        var to = CurrentPurchaseObject.transform.eulerAngles + Vector3.up * 90;
        while (elapsed < rotateduration)
        {
            float t = elapsed / rotateduration;
            CurrentPurchaseObject.transform.eulerAngles=Vector3.Lerp(CurrentPurchaseObject.transform.eulerAngles, to, t);
            //onUpdate?.Invoke(current);
            elapsed += Time.deltaTime;
            yield return null;
        }
        CurrentPurchaseObject.transform.eulerAngles = to;
        // Ensure the final value is exactly 'to'
        //onUpdate?.Invoke(to);
        //onComplete?.Invoke();
    }
    public void ConfirmPurchas()
    {
        //TODO: currency Deduction here

        //Disabling Building Hud
        buildingHud.gameObject.transform.SetParent(this.gameObject.transform);
        buildingHud.gameObject.SetActive(false);
        CurrentPurchaseObject.GetComponent<Collider>().isTrigger = false;
        //Init the Build Object
        if (CurrentPurchaseObject.GetComponent<InteractiveBlock>() != null)
        {CurrentPurchaseObject.GetComponent<InteractiveBlock>().Init(EquipmentType.none,_currentPrefabID);}
        //Confirm the build location
        InputManager.Instance.UpdateGridPositions(CurrentPurchaseObject.transform.position, 
            CurrentPurchaseObject.GetComponent<InteractiveBlock>().GetLookPos().position);
        CurrentPurchaseObject = null;
        //Exit Building mode
        InputManager.Instance.ChangeMode(CameraTargetMode.PlayerCam, GameDataDNDL.Instance.GetPlayer().gameObject);
        InputManager.Instance.ExitBuildingMode();
        isBuildMode = false;
    }
    
    public GameObject PlaceEquipmentAtaPoint(string _prefab,Vector3 position,Vector3 rotation)
    {
        var go = Instantiate(AssetLoader.Instance.GetEquipmetPrefab(_prefab), position, Quaternion.Euler(rotation));
        if (go.GetComponent<InteractiveBlock>() != null)
        { go.GetComponent<InteractiveBlock>().Init(EquipmentType.none, _prefab); }
        //Confirm the build location
        InputManager.Instance.UpdateGridPositions(go.transform.position,
            go.GetComponent<InteractiveBlock>().GetLookPos().position);
        return go;
    }
    IEnumerator CheckConfirmBTN()
    {
        while (isBuildMode) { 
        var t = CurrentPurchaseObject.GetComponent<InteractiveBlock>();
        yield return null;
            if(t!=null)
            buildingHud.ConfirmButton.interactable= InputManager.Instance.CanPlaceHere(t.transform.position,t.GetLookPos().position);
        }
    }
}
