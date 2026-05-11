using UnityEngine;
using Util;

public class MainMenuDNDL : Singletonref<MainMenuDNDL>
{
    public GameObject HUD;
    public GameObject SettingTab;
    public GameObject Credits;
    public GameObject Loading;
    public Animator Animator;
    public void init()
    {
        Animator.Play("Hud Start Animations");
    }
    public void StartCurrentSave()
    {
        HUD.SetActive(false);
    }
    public void NewSave()
    {
        //HUD.SetActive(false);
        SceneManagerDNDL.Instance.ReloadScene(SceneManagerDNDL.Instance.GameScene, () => { });
        GameDataDNDL.Instance.NewSave();
    }
    public void Settings()
    {

    }
    public void Quit()
    {
        Application.Quit();
        
    }
}
