using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEngine;

public class IconGen : MonoBehaviour
{
    public Camera cam;
    private Texture2D m_PreviewTexture;
    int m_size = 1024;
    private void Start()
    {
        if (m_PreviewTexture == null)
        {
            m_PreviewTexture = new Texture2D(m_size, m_size, TextureFormat.RGBAFloat, false);
        }
        RenderTexture.active = cam.targetTexture;
        m_PreviewTexture.ReadPixels(new Rect(0, 0, m_size, m_size), 0, 0);
        m_PreviewTexture.Apply();

        RenderTexture.active = null;
    }

#if UNITY_EDITOR
    private void SaveTextureAsPNG(Texture2D texture, string filename)
    {
        if (texture == null)
        {
            Debug.LogError("No texture provided to save.");
            return;
        }

        string path = EditorUtility.SaveFilePanel(
            "Save Texture As PNG",
            "",
            $"{filename}.png",
            "png"
        );

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("Save operation cancelled.");
            return;
        }

        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            File.WriteAllBytes(path, pngData);
            Debug.Log("Texture saved to: " + path);
        }
        else
        {
            Debug.LogError("Failed to encode texture to PNG.");
        }
    }
#endif
}
