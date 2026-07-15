//using System.IO;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using UnityEditor.Toolbars;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class MainToolbarDropdownSceneSelectorExample
//{
//    const string kElementPath = "Examples/Scene Selector";

//    static string[] scenePaths;

//    [MainToolbarElement(kElementPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
//    public static MainToolbarElement CreateSceneSelectorDropdown()
//    {
//        string activeSceneName;
//        if (Application.isPlaying)
//            activeSceneName = SceneManager.GetActiveScene().name;
//        else
//            activeSceneName = EditorSceneManager.GetActiveScene().name;
//        if (activeSceneName.Length == 0)
//            activeSceneName = "Untitled";

//        var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
//        var content = new MainToolbarContent(activeSceneName, icon, "Select active scene");
//        return new MainToolbarDropdown(content, ShowDropdownMenu);
//    }

//    static void ShowDropdownMenu(Rect dropDownRect)
//    {
//        var menu = new GenericMenu();
//        if (scenePaths.Length == 0)
//        {
//            menu.AddDisabledItem(new GUIContent("No Scenes in Project"));
//        }
//        foreach (string scenePath in scenePaths)
//        {
//            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
//            menu.AddItem(new GUIContent(sceneName), false, () =>
//            {
//                SwitchScene(scenePath);
//            });
//        }
//        menu.DropDown(dropDownRect);
//    }

//    static void SwitchScene(string scenePath)
//    {
//        if (Application.isPlaying)
//        {
//            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
//            if (Application.CanStreamedLevelBeLoaded(sceneName))
//            {
//                Debug.Log($"Switching to scene: {sceneName}");
//                SceneManager.LoadScene(sceneName);
//            }
//            else
//            {
//                Debug.LogError($"Scene '{sceneName}' is not in the Build Settings.");
//            }
//        }
//        else
//        {
//            if (File.Exists(scenePath))
//            {
//                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
//                {
//                    Debug.Log($"Switching to scene: {scenePath}");
//                    EditorSceneManager.OpenScene(scenePath);
//                }
//            }
//            else
//            {
//                Debug.LogError($"Scene at path '{scenePath}' does not exist.");
//            }
//        }
//    }

//    static void RefreshSceneList()
//    {
//        scenePaths = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);
//    }

//    static void SceneSwitched(Scene oldScene, Scene newScene)
//    {
//        MainToolbar.Refresh(kElementPath);
//    }

//    static MainToolbarDropdownSceneSelectorExample()
//    {
//        RefreshSceneList();
//        EditorApplication.projectChanged += RefreshSceneList;
//        SceneManager.activeSceneChanged += SceneSwitched;
//        EditorSceneManager.activeSceneChangedInEditMode += SceneSwitched;
//    }
//}
