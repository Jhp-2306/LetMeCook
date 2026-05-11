using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using static UnityEngine.Rendering.HDROutputUtils;


public class SceneManagerDNDL : Singletonref<SceneManagerDNDL>
{
    public GameObject SplashScreen;
    public SceneField MainMenu;
    public SceneField GameScene;
    public SceneField GameLevelDesign;
    public List<AsyncOperation> operations;
    private void Awake()
    {
        base.Awake();
        StartCoroutine(Starter());
    }
    IEnumerator Starter()
    {
        yield return new WaitForSeconds(3f);
        operations = new List<AsyncOperation>();
        operations.Add(SceneManager.LoadSceneAsync(GameLevelDesign, LoadSceneMode.Additive));
        operations.Add(SceneManager.LoadSceneAsync(GameScene, LoadSceneMode.Additive));
        operations.Add(SceneManager.LoadSceneAsync(MainMenu, LoadSceneMode.Additive));
        yield return new WaitUntil(()=>operations[operations.Count - 1].isDone);
        SplashScreen.SetActive(false);
        GameObject.FindAnyObjectByType<MainMenuDNDL>().init();
        yield return new WaitForSeconds(3f);
    }
    public void ReloadScene(SceneField scene,Action Callback)
    {
        StartCoroutine(ReloadSceneCor(scene, Callback));   
    }
    IEnumerator ReloadSceneCor(SceneField scene, Action Callback) {
        var temp= SceneManager.UnloadSceneAsync(scene);
        yield return new WaitUntil(() => temp.isDone);
        temp=SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        yield return new WaitUntil(() => temp.isDone);
    }


}
