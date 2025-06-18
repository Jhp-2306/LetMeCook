using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemHolder : MonoBehaviour
{
    public Image Icon;
    public TMPro.TextMeshProUGUI Count;
    public TMPro.TextMeshProUGUI Title;
    SO_IngredientData details;


    public void Set(SO_IngredientData data,string _count,string name)
    {
        details = data;
        Icon.sprite = details.icon;
        Count.text = _count;
        Title.text = name;
    }

    public void onclick()
    {
        var go = Instantiate(AssetLoader.Instance.itemPrefab);
        go.GetComponent<Ingredient>().Setup(details.mesh,details.type,details.handheldtype);
        GameDataDNDL.Instance.GetPlayer().PickSomeThing(go.GetComponent<IHandHeld>(), go);
    }
}
