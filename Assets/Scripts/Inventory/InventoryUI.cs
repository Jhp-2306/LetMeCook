using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject Row1, Row2, Row3;
    
    public List<GameObject> Items;

    
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
       
        for (int i = 0; i < Items.Count; i++) {
            if (items.GetAllItems().Count - 1 >= i)
            {
                Items[i].SetActive(true);
                var item = items.GetItemAtIndex(i);
                Items[i].GetComponent<InventoryItemHolder>().Set(AssetLoader.Instance.GetIngredientSO(item.Type), 
                    item.GetQuanitity().ToString(),item.Type.ToString(),()=> { item.RemoveQuanitity();
                    Close();
                });
            }
            else
            {
                Items[i].SetActive(false);
            }
        }
    }

    public void Close()
    {
        HUDManagerDNDL.Instance.CloseInventory();
    }
}
