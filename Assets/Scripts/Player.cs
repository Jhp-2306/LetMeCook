using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    IHandHeld inHand;

    public Transform HandParent;

    public bool isHandsfull;
    #region Property
    public bool isPlayerHandEmpty => inHand == null;
    public IHandHeld InHand { get=>inHand; }
    #endregion

    public void PickSomeThing(IHandHeld _inHand,GameObject obj)
    {
        inHand = _inHand;
        obj.transform.SetParent(HandParent);
        obj.transform.localPosition = Vector3.zero;
        isHandsfull=true;
    }
    public void RemoveFromHand()
    {
        inHand.DestroyMe();
        inHand = null;
        isHandsfull = false;
    }
    private void Start()
    {
        SetPlayerToGameData();
    }
    private void OnEnable()
    {
        SetPlayerToGameData();
    }
    void SetPlayerToGameData()
    {
         if(GameDataDNDL.HasInstance()) { GameDataDNDL.Instance.SetPlayer(this); }

    }

}

