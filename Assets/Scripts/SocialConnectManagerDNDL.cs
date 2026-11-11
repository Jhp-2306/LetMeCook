using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Util;
using Facebook.Unity;
using System;
using System.Collections.Generic;
using Facebook.Unity.Example;



#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
public class SocialConnectManagerDNDL : Singletonref<SocialConnectManagerDNDL>
{
    private string m_GPG_Token;
    private async void Awake()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.DebugLogEnabled=true;
        PlayGamesPlatform.Activate();
        LoginGooglePlayGames();
#endif
        InitializeFacebook();
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log("Auth init");
            await UnityServices.InitializeAsync();
            await SignUpAnonymouslyAsync();
        }
    }

    
    async void Start()
    {
       
    }
    public void OpenYouTube()
    {
#if UNITY_EDITOR
        Application.OpenURL("https://www.youtube.com/@NobodyIsJHP");
#elif UNITY_ANDROID
        OpenAndroid("vnd.youtube://@NobodyIsJHP", "https://www.youtube.com/@NobodyIsJHP");
#elif UNITY_IOS
        OpenIOS("youtube://@NobodyIsJHP", "https://www.youtube.com/@NobodyIsJHP");
#endif
    }

    public void OpenTwitch()
    {
#if UNITY_EDITOR
        Application.OpenURL("https://www.twitch.tv/nobodyisjhp");
#elif UNITY_ANDROID
        OpenAndroid("twitch://nobodyisjhp", "https://www.twitch.tv/nobodyisjhp");
#elif UNITY_IOS
        OpenIOS("twitch://nobodyisjhp", "https://www.twitch.tv/nobodyisjhp");
#endif
    }

#if UNITY_ANDROID

    private void OpenAndroid(string appUrl, string fallbackUrl)
    {
        using (AndroidJavaClass unityPlayer =
               new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            using (AndroidJavaClass uriClass =
                   new AndroidJavaClass("android.net.Uri"))
            using (AndroidJavaClass intentClass =
                   new AndroidJavaClass("android.content.Intent"))
            {
                AndroidJavaObject uri =
                    uriClass.CallStatic<AndroidJavaObject>(
                        "parse",
                        appUrl
                    );

                AndroidJavaObject intent =
                    new AndroidJavaObject(
                        "android.content.Intent",
                        "android.intent.action.VIEW",
                        uri
                    );

                try
                {
                    activity.Call("startActivity", intent);
                }
                catch
                {
                    Application.OpenURL(fallbackUrl);
                }
            }
        }
    }

#endif

#if UNITY_IOS

    private void OpenIOS(string appUrl, string fallbackUrl)
    {
        Application.OpenURL(appUrl);

        // iOS does not provide a reliable synchronous way
        // to know whether the app actually opened.
        Invoke(nameof(OpenFallback), 1.0f);

        fallbackURL = fallbackUrl;
    }

    private string fallbackURL;

    private void OpenFallback()
    {
        if (!Application.isFocused)
            return;

        Application.OpenURL(fallbackURL);
    }

#endif

    #region GuestPlay
    async Task SignUpAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    #endregion
    #region Facebook
    private void InitializeFacebook()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log(CustomLogs.CC_TagLog("Social Manager(Facebook)", "Failed to init the fb sdk"));
        }
    }

    private void OnHideUnity(bool isUnityShown)
    {
        if (!isUnityShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    public void StartFacebookSignIn()
    {
        var perms = new List<string>()
        {"public_profile","email" };
        FB.LogInWithReadPermissions(perms, async result =>
        {
            if (FB.IsLoggedIn)
            {
                var accessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await SignInWithFacebookAsync(accessToken);
                }
                else
                {
                    await LinkWithFacebookAsync(accessToken);
                }
            }
            else
            {
                Debug.Log(CustomLogs.CC_TagLog("Social Manager(Facebook)", "user Cancelled login"));
            }
        });
    }

    private async Task LinkWithFacebookAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithFacebookAsync(accessToken);
            Debug.Log(CustomLogs.CC_TagLog("Social Manager(Facebook)", "Linked in with Facebook"));
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.LogException(ex);
        }
        catch (Exception ex)

        { Debug.LogException(ex); }
    }

    private async Task SignInWithFacebookAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithFacebookAsync(accessToken);
            Debug.Log(CustomLogs.CC_TagLog("Social Manager(Facebook)", "Signed in with Facebook"));
        }
        catch (RequestFailedException e)
        {
            Debug.LogException(e);
        }
    }
    #endregion

    private void LoginGooglePlayGames()
    {
        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
                    Debug.Log(CustomLogs.CC_TagLog("Social Manager(Google Play)", $"Login with Google Play successful"));
            if (status == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code => {
                    Debug.Log(CustomLogs.CC_TagLog("Social Manager(Google Play)", $"auth code{code}"));
                    m_GPG_Token = code;
                });
            }
            else
            {
                    Debug.Log(CustomLogs.CC_TagLog("Social Manager(Google Play)", $"Login with Google Play unsuccessful: {status}"));

            }
        });
    }
    public void StartSignInWithGooglePlay()
    {
        if (!PlayGamesPlatform.Instance.IsAuthenticated())
        {
            Debug.Log(CustomLogs.CC_TagLog("Social Manager(Google Play)", $"no auth with Google Play --starting to login again "));
            LoginGooglePlayGames();
            return;
        }
        SigninOrLinkWithGooglePlayGames();
    }

    private async void SigninOrLinkWithGooglePlayGames()
    {
        if (string.IsNullOrEmpty(m_GPG_Token))
        {
            Debug.Log(CustomLogs.CC_TagLog("Social Manager(Google Play)", $"auth token is null"));
            return;
        }
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await SignInWithGooglePlayGamesAsync(m_GPG_Token);
        }
        else
        {
            await LinkWithGooglePlayGamesAsync(m_GPG_Token);
        }
    }

    private async Task LinkWithGooglePlayGamesAsync(string auth)
    {
      try
        {
            await AuthenticationService.Instance.LinkWithGooglePlayGamesAsync(auth);
            Debug.Log(CustomLogs.CC_TagLog("Social Manager(Google Play)", "Linked in with Google Play"));
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.LogException(ex);
        }
        catch (AuthenticationException e)
        {
            Debug.LogException(e);
        }
        catch (RequestFailedException e)
        {
            Debug.LogException(e);
        }
    }

    private async Task SignInWithGooglePlayGamesAsync(string auth)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(auth);
            Debug.Log(CustomLogs.CC_TagLog("Social Manager(Google Play)", $"Google PlaySign in Successful"));
        }
        catch (AuthenticationException e)
        {
            Debug.LogException(e);
        }
        catch (RequestFailedException e) {
            Debug.LogException(e);
        }
    }
}
