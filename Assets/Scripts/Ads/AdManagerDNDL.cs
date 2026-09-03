using System;
using UnityEngine;
using Util;
public class AdManagerDNDL : Singletonref<AdManagerDNDL>
{
    [SerializeField] private RewardedAds ads;
    [SerializeField] private bool isTesting;
    public bool IsTesting{ get=>isTesting; }
    private void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        ads.LoadAd();
    }
    public void ShowAds(Action _successCallback,Action _failedCallback)
    {
        ads.ShowAd(_successCallback,_failedCallback);
    }
}

