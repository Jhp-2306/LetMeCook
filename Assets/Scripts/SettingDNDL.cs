using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Util;

public class SettingDNDL : Singletonref<SettingDNDL>
{
    [Range(0f,1f)]
    public float SFXVolume;
    [Range(0f,1f)]
    public float MusicVolume;
    
    private bool adsNotification;
    private bool localNotification;
    private bool pushNotification;
    private bool ftut;
    public bool AdsNotification { get=>adsNotification;  }
    public bool LocalNotification { get=>localNotification;  }
    public bool PushNotification { get=>pushNotification;  }
    public bool FTUT { get => ftut; }
    public Slider Sfx;
    public Slider Music;
    public Toggle AdsToggle, localNotifyToggle, PushNotifyToggle, FtutDisableToggle;

    public GameObject MainMenuBtn;
    private void Awake()
    {
         base.Awake();
         //DontDestroyOnLoad(this);
         //this.gameObject.SetActive(false);
    }
    private void Start()
    {
        this.gameObject.SetActive(false);
        SFXVolume =         GameSettingsData.Instance.setting.SFXVolume;
        MusicVolume =       GameSettingsData.Instance.setting.MusicVolume;
        adsNotification=    GameSettingsData.Instance.setting.AdsNotify;
        localNotification = GameSettingsData.Instance.setting.LocalNotify;
        pushNotification=   GameSettingsData.Instance.setting.PushNotify;
        ftut =              GameSettingsData.Instance.setting.TutorialDisable;
        AdsToggle.onValueChanged.AddListener((val) => { adsNotification = val; });
        localNotifyToggle.onValueChanged.AddListener((val) => { localNotification = val; });
        PushNotifyToggle.onValueChanged.AddListener((val) => { pushNotification = val; });
        FtutDisableToggle.onValueChanged.AddListener((val) => { ftut = val; });
        Sfx.onValueChanged.AddListener((val) =>
        {
            SFXVolume = val;
            GameMusicNSoundsDNDL.Instance.SfxAudioChannel.volume = SFXVolume;
        });
        Music.onValueChanged.AddListener((val) =>
        {
            MusicVolume = val;
            GameMusicNSoundsDNDL.Instance.MusicAudioChannel.volume = MusicVolume;
        });
        Debug.Log("Done with settings");
    }
    public void OpenSettings()
    {
        Debug.Log(this.gameObject.name);
        Sfx.value = SFXVolume;
        Music.value = MusicVolume;
        AdsToggle.isOn = adsNotification;
        localNotifyToggle .isOn = localNotification;
        PushNotifyToggle  .isOn = pushNotification;
        FtutDisableToggle .isOn = ftut;

        MainMenuBtn.SetActive(!MainMenuDNDL.Instance.isMainMenu);
        

        this.gameObject.SetActive(true);

    }
    public void Init()
    {

    }
    public void onSfxVolumeChanged(int volume)
    {
        //SFXVolume=Sfx.value;
        //GameMusicNSoundsDNDL.Instance.SfxAudioChannel.volume = SFXVolume;
    }
    public void onMusicVolumeChanged(int volume)
    {
        //MusicVolume = Music.value;
        //GameMusicNSoundsDNDL.Instance.MusicAudioChannel.volume = MusicVolume;
    }
    public void FBSignIN()
    {
        SocialConnectManagerDNDL.Instance.StartFacebookSignIn();
    }
    public void AppleSignIn()
    {

    }
    public void GooglePlaySignIn()
    {
        SocialConnectManagerDNDL.Instance.StartSignInWithGooglePlay();
    }
    public void GotoMainMenu()
    {
        MainMenuDNDL.Instance.init();
        closepopup();
    }
    

    public void closepopup()
    {
        GameMusicNSoundsDNDL.Instance.playClickSFX();
        this.gameObject.SetActive(false);
        //Save Settings
        GameSettingsData.Instance.setting.SFXVolume=SFXVolume;
        GameSettingsData.Instance.setting.MusicVolume = MusicVolume;
        GameSettingsData.Instance.setting.AdsNotify= adsNotification;
        GameSettingsData.Instance.setting.LocalNotify=localNotification;
        GameSettingsData.Instance.setting.PushNotify=pushNotification;
        GameSettingsData.Instance.setting.TutorialDisable=ftut;
        GameSettingsData.Instance.SaveInstance();
    }

}
