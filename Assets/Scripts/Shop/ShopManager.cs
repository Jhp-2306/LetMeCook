using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Constants;
using Unity.VisualScripting;

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
        Sprite icon;
        GameObject Prefab;

        public ShopItems(string name, int price, EquipmentType _type, GameObject _prefab, Sprite icon = null)
        {
            this.name = name;
            this.price = price;
            this.icon = icon;
            this.type = _type;
            this.Prefab = _prefab;
        }
        public string GetName() => name;
        public int GetPrice() => price;
        public Sprite GetIcon() => icon;
        public GameObject GetPrefab() => Prefab;
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
    private void Start()
    {
        Items = new List<ShopItems>();
        if (GameDataDNDL.Instance.ItemList != null)
        {
            foreach (var item in GameDataDNDL.Instance.ItemList.equipmentDataList)
            {
                //Debug.Log(item.Prefab==null);
                var t = new ShopItems(item.name, item.price, item.Type, item.Prefab);
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

    public void BuildModeActivate(GameObject _prefab)
    {
        var templistpos = GameDataDNDL.Instance.GetGrid().GetAvailableNeighbourPosition(GameDataDNDL.Instance.GetPlayer().gameObject.transform.position);
        if (templistpos.Count > 0)
        {
            var position = templistpos[Random.RandomRange(0, templistpos.Count)];
            var go = Instantiate(_prefab, position, Quaternion.identity);
            CurrentPurchaseObject = go;
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
        //Init the Build Object
        if (CurrentPurchaseObject.GetComponent<InteractiveBlock>() != null)
        {CurrentPurchaseObject.GetComponent<InteractiveBlock>().Init();}
        //Confirm the build location
        InputManager.Instance.UpdateGridPositions(CurrentPurchaseObject.transform.position, 
            CurrentPurchaseObject.GetComponent<InteractiveBlock>().GetLookPos().position);
        CurrentPurchaseObject = null;
        //Exit Building mode
        InputManager.Instance.ChangeMode(CameraTargetMode.PlayerCam, GameDataDNDL.Instance.GetPlayer().gameObject);
        InputManager.Instance.ExitBuildingMode();
        isBuildMode = false;
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
