using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Constants;

public class ShopManager : Singletonref<ShopManager>
{
    [SerializeField]
    SO_EquipmentData SO_EquipmentData;
    public SO_EquipmentDataList ItemList;
    public Table CurrentSelectTable;

    public InteractionBTN interactionBTN;

    //public Table GetCurrentTable { get => CurrentSelectTable; }
    public class ShopItems
    {
        EquipmentType type;
        string name;
        int price;
        Sprite icon;
        GameObject Prefab;

        public ShopItems(string name, int price, EquipmentType _type, GameObject _prefab,Sprite icon=null)
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
    private void Start()
    {
        Items = new List<ShopItems>();
        if(ItemList != null) {
            foreach(var item in ItemList.equipmentDataList) {
                //Debug.Log(item.Prefab==null);
                var t = new ShopItems(item.name, item.price,item.Type,item.Prefab);
                Items.Add(t);
            }
            MaxShopItems = Items.Count;
        }
        else
        {
            Debug.LogError("No items in the shop");
        }
    }
    public List<ShopItems> GetItems() => Items;

    public void CloseShop()
    {
        interactionBTN.ShopClose();
    }

    
    
}
