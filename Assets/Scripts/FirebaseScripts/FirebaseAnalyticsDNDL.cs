using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;
using Util;

public class FirebaseAnalyticsDNDL : Singletonref<FirebaseAnalyticsDNDL>
{
    private FirebaseApp app;
    private bool init;
    public bool InitDone { get=>init&&app!=null; }
    private void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        init = false;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;
                init=true;
                Debug.Log(CustomLogs.CC_TagLog("Firebase_Dndl", $"firebase init Done{InitDone}"));
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                // Firebase Unity SDK is not safe to use here.
                init=false;
                Debug.LogError(CustomLogs.CC_TagLog("Firebase_Dndl",
                  string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus)));
            }
        });
        
    }

    public void Log(string message,string Para,string para)
    {
        //FirebaseAnalytics.LogEvent(message, Para, para);    
        FirebaseAnalytics.LogEvent(message, new Parameter[]
        {
            new Parameter(Para,para)
        });

    }
    public void Log(string message, Parameter[] Para)
    {
        FirebaseAnalytics.LogEvent(message, Para);

    }


}
