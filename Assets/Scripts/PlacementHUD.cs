using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementHUD : MonoBehaviour
{
    public Button ConfirmButton;
    public void OnConfirme()
    {
        ShopManager.Instance.ConfirmPurchas();
    }
    public void OnCancel()
    {
        ShopManager.Instance.CancelPurchase();
    }
    public void OnRotate()
    {
        ShopManager.Instance.RotateObject();
    }
    
}
