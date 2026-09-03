using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAds : MonoBehaviour,IUnityAdsInitializationListener,IUnityAdsLoadListener,IUnityAdsShowListener
{
    [SerializeField]private string androidAdID;
    [SerializeField]private string iosAdID;
    //[SerializeField]private bool isTesting;

    private string advertisementID;

    private const string _AndroidRewardAdUnitID = "Rewarded_Android";
    private const string _iOSRewardAdUnitID = "Rewarded_iOS";
    private string _RewardAdUnitID;
    private Action RewardAdCallback;
    private Action RewardAdFailedCallback;

    #region init Callback
    public void OnInitializationComplete()
    {
        Debug.Log(CustomLogs.CC_TagLog("Ads", "Ads init Successful"));
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {

    }
    #endregion
    #region Ad Load Callbacks
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log(CustomLogs.CC_TagLog("Ads", "Rewarded Ads Loaded Successful"));
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log(CustomLogs.CC_TagLog("Ads", $"Rewarded Ads Failed to Load{message}"));
    }
    #endregion
    #region Ad Show Callbacks
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log(CustomLogs.CC_TagLog("Ads", $"Rewarded Ads Failed to show{message}"));
        RewardAdFailedCallback();
        RewardAdFailedCallback = delegate { };
    }

    public void OnUnityAdsShowStart(string placementId)
    {
    }

    public void OnUnityAdsShowClick(string placementId)
    {
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if(placementId==_RewardAdUnitID&&showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED)) {
        Debug.Log(CustomLogs.CC_TagLog("Ads", "Rewarded Ads completed Successfully"));
            //Give rewards here
            RewardAdCallback();
            RewardAdCallback = delegate { };
        }
    }
    #endregion


    private void Awake()
    {
#if UNITY_ANDROID
        advertisementID=androidAdID;
        _RewardAdUnitID = _AndroidRewardAdUnitID;
#elif UNITY_IOS
        advertisementID=iosID;
        RewardAdUnitID = _iOSRewardAdUnitID;
#endif
        RewardAdFailedCallback = delegate { };
        RewardAdCallback = delegate { };
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(advertisementID, AdManagerDNDL.Instance.IsTesting, this);
        }
    }

    public void LoadAd()
    {
        Advertisement.Load(_RewardAdUnitID, this);
    }
    public void ShowAd(Action _callback,Action _failedCallback)
    {
        RewardAdCallback= _callback;
        RewardAdFailedCallback= _failedCallback;    
        Advertisement.Show(_RewardAdUnitID, this);
        LoadAd();
    }

   
}
