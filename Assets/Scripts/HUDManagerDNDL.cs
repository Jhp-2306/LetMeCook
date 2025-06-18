using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class HUDManagerDNDL : Singletonref<HUDManagerDNDL>
{
    public InteractionBTN interactionBTN;
    BasicStorageSystem<IStorageItem> currentOpenStorage;
    [Header("Toast Message")]
    public GameObject ToastGameObject;
    public TMPro.TextMeshProUGUI ToastMsg;
    Coroutine Toast;
    public void CloseShop()
    {
        interactionBTN.ShopClose();
    }
    public void OpenInventory(BasicStorageSystem<IStorageItem> items)
    {
        currentOpenStorage = items;
        interactionBTN.OpenInventory(items);
        //interactionBTN.ResetButton();
    }
    public void CloseInventory()
    {
        interactionBTN.CloseInventory();
    }

    public void SetProgressBar(float sec, Action callback)
    {
        //interactionBTN.SetProgressBar(val);
        StartCoroutine(interactionBTN.TimerProgressBar(sec, callback));

    }
    public void ShowToastMsg(string msg) {
        if (Toast == null) { 
        Toast=StartCoroutine(ShowToast(3,msg));
        }
        else
        {
            StopCoroutine(Toast);
            Toast = StartCoroutine(ShowToast(3, msg));
        }
    }
    IEnumerator ShowToast(float maxTimer, string msg)
    {
        var timer = 0f;
        ToastGameObject.SetActive(true);
        ToastMsg.text = msg;
        while (timer <= maxTimer)
        {
            yield return null;
            timer += Time.deltaTime;
            //if (timer > maxTimer)
        }
        ToastGameObject.SetActive(false);
        ToastMsg.text = "";

    }
}
