using System;
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
    Action Callback;

    public void Set(SO_IngredientData data,string _count,string name,Action callback)
    {
        details = data;
        Icon.sprite = details.icon;
        Count.text = _count;
        Title.text = name;
        Callback=callback;
    }

    public void onclick()
    {
        var player = GameDataDNDL.Instance.GetPlayer();
        if (player.isHandsfull) return;
        //Reduce the total count;
        Callback?.Invoke();
        var go = Instantiate(AssetLoader.Instance.itemPrefab);
        go.GetComponent<Ingredient>().Setup(details.mesh,details.type,details.handheldtype,details.material);
        player.PickSomeThing(go.GetComponent<IHandHeld>(), go);
    }
}
