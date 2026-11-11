using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public GameObject ShopParent;
    public GameObject Prefab;
    List<ShopItemHolderUI> Equipments_items;
    List<ShopItemHolderUI> Furnitures_items;
    public void init()
    {
        Equipments_items = new List<ShopItemHolderUI>();
        Furnitures_items = new List<ShopItemHolderUI>();
        foreach (var t in ShopManager.Instance.GetItems())
        {
            var go = Instantiate(Prefab, ShopParent.transform);
            //var sprite = AssetLoader.Instance.GetIcons(t.GetIcon());
            go.GetComponent<ShopItemHolderUI>().Setup(t.GetName(), t.GetPrice(), t.GetPrefab(), t.GetEquipmentType(), t.GetIcon());
            Equipments_items.Add(go.GetComponent<ShopItemHolderUI>());
        }
        foreach (var t in ShopManager.Instance.GetFurnitureItems())
        {
            var go = Instantiate(Prefab, ShopParent.transform);
            //var sprite = AssetLoader.Instance.GetIcons(t.GetIcon());
            go.GetComponent<ShopItemHolderUI>().Setup(t.GetName(), t.GetPrice(), t.GetPrefab(), t.GetEquipmentType(), t.GetIcon());
            Furnitures_items.Add(go.GetComponent<ShopItemHolderUI>());
        }
    }
    public void SetupShop(List<ShopManager.ShopItems> items)
    {
        //if (items == null||_items.Count>0) return;

        foreach (var item in Equipments_items)
        {
            item.gameObject.SetActive(GameDataDNDL.Instance.CurrentArea == Constants.PlayableAreas.Kitchen);
            if (GameDataDNDL.Instance.CurrentArea == Constants.PlayableAreas.Kitchen)
            {
                item.RequestIcons();
            }
        }
        foreach (var item in Furnitures_items)
        {
            item.gameObject.SetActive(GameDataDNDL.Instance.CurrentArea == Constants.PlayableAreas.Room);
            if (GameDataDNDL.Instance.CurrentArea == Constants.PlayableAreas.Room)
                item.RequestIcons();
        }
    }
    public void CloseShop()
    {
        HUDManagerDNDL.Instance.CloseShop();
    }
    public void OpenShop()
    {
        HUDManagerDNDL.Instance.OpenShop();
    }
}
