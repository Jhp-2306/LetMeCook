using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public GameObject ShopParent;
    public GameObject Prefab;
    List<ShopItemHolderUI> _items;
    public void init()
    {
        _items = new List<ShopItemHolderUI>();
    }
    public void SetupShop(List<ShopManager.ShopItems> items)
    {
        if (items == null||_items.Count>0) return;
        foreach(var t in items)
        {
            var go = Instantiate(Prefab, ShopParent.transform);
            go.GetComponent<ShopItemHolderUI>().Setup(t.GetName(), t.GetPrice(), t.GetPrefab(), t.GetEquipmentType());
            _items.Add(go.GetComponent<ShopItemHolderUI>());
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
