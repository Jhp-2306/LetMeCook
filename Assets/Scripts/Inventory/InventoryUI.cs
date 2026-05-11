using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject Row1, Row2, Row3;
    
    public List<GameObject> Items;

    public TextMeshProUGUI Slots;
    
    private void Awake()
    {
        Items = new List<GameObject>();
        foreach (var item in Row1.GetComponentsInChildren<InventoryItemHolder>())
        {
            Items.Add(item.gameObject);
        }
        foreach (var item in Row2.GetComponentsInChildren<InventoryItemHolder>())
        {
            Items.Add(item.gameObject);
        }
        foreach (var item in Row3.GetComponentsInChildren<InventoryItemHolder>())
        {
            Items.Add(item.gameObject);
        }
        //Set();
    }

    public void Set(BasicStorageSystem<IStorageItem> items)
    {
        int currentSlotsFilled = 0;
        for (int i = 0; i < Items.Count; i++) {
            if (items.GetAllItems().Count - 1 >= i)
            {
                Items[i].SetActive(true);
                var item = items.GetItemAtIndex(i);
                Items[i].GetComponent<InventoryItemHolder>().Set(AssetLoader.Instance.GetIngredientSO(item.Type), 
                    item.GetQuanitity().ToString(),item.Type.ToString(),()=> { item.RemoveQuanitity();
                    Close();
                });
                currentSlotsFilled++;
            }
            else
            {
                Items[i].SetActive(false);
            }
        }
        Slots.text = $"{currentSlotsFilled}/{items.GetSlotsCapcity()}";
    }

    public Transform GetItemSlots()
    {
        return Items[0].transform;
    }
    public void Close()
    {
        HUDManagerDNDL.Instance.CloseInventory();
    }
}
