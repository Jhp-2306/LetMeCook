using UnityEngine;
using Util;

public class MainMenuDNDL : Singletonref<MainMenuDNDL>
{
    public GameObject HUD;
    public GameObject SettingTab;
    public GameObject Credits;
    public GameObject Loading;
    public Animator Animator;
    public bool isMainMenu;
    public void init()
    {
        Animator.Play("Hud Start Animations");
        GameMusicNSoundsDNDL.Instance.StartMusic();
        HUD.SetActive(true);
        isMainMenu = true;
    }
    public void StartCurrentSave()
    {
        GameMusicNSoundsDNDL.Instance.playClickSFX();
        HUD.SetActive(false);
        GameDataDNDL.Instance.ContinueSave() ;
        isMainMenu = false;
    }
    public void NewSave()
    {
        //HUD.SetActive(false);
        //TODO:Show Load Scene
        GameMusicNSoundsDNDL.Instance.playClickSFX();
        GameSaveDNDL.Instance.NewGame();
        SceneManagerDNDL.Instance.ReloadScene(SceneManagerDNDL.Instance.GameScene, () => { HUD.SetActive(false);
            StartCurrentSave();
        });
        isMainMenu = false;
        //HUD.SetActive(false);
        //GameDataDNDL.Instance.NewSave();
    }
    public void Settings()
    {
        GameMusicNSoundsDNDL.Instance.playClickSFX();
        SettingDNDL.Instance.OpenSettings();
    }
    public void Quit()
    {
        GameMusicNSoundsDNDL.Instance.playClickSFX();
        Application.Quit();
        
    }
}
