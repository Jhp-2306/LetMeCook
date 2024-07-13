using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelEditorGridData))]
public class LevelEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var t = (LevelEditorGridData)target;
        if(GUILayout.Button("Generate Grid"))
        {
            t.CreateGridobjects();
        }
        if (GUILayout.Button("Clear Grid"))
        {
            t.cleartheGrid();
        }
        if (GUILayout.Button("Color Grid"))
        {
            t.colortheSector();
        }
    }
}
