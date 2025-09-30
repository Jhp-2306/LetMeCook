using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementHUD : MonoBehaviour
{
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
