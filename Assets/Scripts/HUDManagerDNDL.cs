using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [Header("Shop")]
    [SerializeField] GameObject ShopParent;
    [SerializeField] ShopUI ShopUI;
    [SerializeField] GameObject ShopButton;
    bool isShopOpen;

    bool isplayerCam;
    const string s_PlayerCam = "Player Cam";
    const string s_FreeCam = "Free Cam";
    public TextMeshProUGUI CameraSettingButton;

    public FillUpMiniGame MiniGame;
    public void OnCameraSettingButtonClicked()
    {
        isplayerCam=!isplayerCam;
        CameraSettingButton.text=isplayerCam?s_FreeCam:s_PlayerCam;
        //Call the Camera Check Method here
        InputManager.Instance.ChangeMode(isplayerCam?CameraTargetMode.PlayerCam:CameraTargetMode.FreeCam, 
            isplayerCam ? GameDataDNDL.Instance.GetPlayer().gameObject : GameDataDNDL.Instance.GetFreeCamRig());
    }
    #region Shop
    public void OpenShop()
    {
        //interactionBTN.ShopOpen();
        ShopUI.SetupShop(ShopManager.Instance.GetItems());
        ShopParent.SetActive(true);
        isShopOpen = true;
    }
    public void CloseShop()
    {
        ShopParent.SetActive(false);
        isShopOpen = false;
        //interactionBTN.ShopClose();
    }
    public void ShopDisable()
    {
        if(isShopOpen) CloseShop();
        ShopButton.SetActive(false);
    }
    public void ShopEnable()
    {
        //if (isShopOpen) CloseShop();
        ShopButton.SetActive(true);
    }
    #endregion
    #region Inventory 

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

    #endregion
    #region ProgressBar
    public void SetProgressBar(float sec, Action callback)
    {
        //interactionBTN.SetProgressBar(val);
        interactionBTN.TimerProgressBar(sec, callback);

    }
    #endregion
    #region Toast Msg
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
    #endregion
}
