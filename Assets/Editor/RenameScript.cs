
using UnityEngine;
using UnityEditor;

public class RenameScript : EditorWindow
{
    string prefix = "NewWord_";

    [MenuItem("Tools/Add Prefix to Selected")]
    public static void ShowWindow()
    {
        GetWindow<RenameScript>("Add Prefix");
    }

    void OnGUI()
    {
        prefix = EditorGUILayout.TextField("Prefix", prefix);
        if (GUILayout.Button("Add Prefix"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.name = prefix + obj.name;
            }
        }
    }
}