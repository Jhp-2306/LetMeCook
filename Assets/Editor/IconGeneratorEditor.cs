
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
//using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class IconGeneratorEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/IconGeneratorEditor")]
    public static void ShowExample()
    {
        IconGeneratorEditor wnd = GetWindow<IconGeneratorEditor>();
        wnd.titleContent = new GUIContent("IconGeneratorEditor");
    }

    private ListView m_list;
    private List<GameObject> m_props;

    [SerializeField]
    private GameObject m_Selectedobj;
    [SerializeField]
    private Texture2D m_PreviewTexture;
    [SerializeField]
    private RenderTexture m_Preview;

    //Preview Scene 
    private int m_size = 1024;
    private UnityEngine.SceneManagement.Scene m_PreviewScene;
    private GameObject m_CamerObject;
    private GameObject m_instance;
    private Light m_dirLight;
    private Camera m_SceneCamera;
    private Button m_save;
    [SerializeField]
    private UnityEditor.UIElements.ObjectField m_parent;
    private UnityEditor.UIElements.ObjectField m_cam;
    private GameObject parent;
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
        m_list = rootVisualElement.Q<ListView>("list");
        m_props = new List<GameObject>();
        string[] files = { "Assets", };
        string[] assetGuids = AssetDatabase.FindAssets("t:Prefab",files);
        foreach (string guid in assetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var temp = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            m_props.Add(temp);
        }
        //parent = new GameObject();
        m_list.itemsSource = m_props;
        m_list.selectionChanged += onSelectItem;
        rootVisualElement.Q<VisualElement>("content").dataSource = this;
        m_cam = rootVisualElement.Q<UnityEditor.UIElements.ObjectField>("Cam");
        m_cam.objectType = typeof(Camera);

       

        m_parent = rootVisualElement.Q<UnityEditor.UIElements.ObjectField>("parentGO");
        Debug.Log(m_parent == null);
        m_parent.objectType = typeof(GameObject);
        m_save = rootVisualElement.Q<Button>("saveas");
        m_save.clicked += Testing;
        m_parent.RegisterValueChangedCallback(OnValueChanges);
        m_parent.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) => OnValueChanges(evt));
        m_cam.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) => { m_SceneCamera = (Camera)evt.newValue;
            m_SceneCamera.targetTexture = new RenderTexture(m_size, m_size, 32, RenderTextureFormat.ARGBFloat);
            m_SceneCamera.aspect = (float)m_size / m_size;
            UpdateCamera();
        });

        
        //rootVisualElement.Add(m_parent);

    }

    

    private void OnValueChanges(ChangeEvent<UnityEngine.Object> evt)
    {
        Debug.Log("New object assigned: " + evt.newValue?.name);
        //Debug.Log($"value changes{parent}" + parent == null);
        parent=(GameObject)evt.newValue;
    }

    private void Update()
    { 
       UpdateCamera();
    }
    private void onSelectItem(object item)
    {
        m_Selectedobj = m_props[m_list.selectedIndex];
        Debug.Log(m_Selectedobj.name);
        if (!m_PreviewScene.IsValid())
        {
            //m_PreviewScene = EditorSceneManager.NewPreviewScene();
            m_PreviewScene = SceneManager.GetActiveScene();
        }
        if (m_instance != null)
        {
            DestroyImmediate(m_instance);
        }
        //Debug.Log((Transform)m_parent.value==null);
        m_instance = (GameObject)PrefabUtility.InstantiatePrefab(m_Selectedobj, parent.transform);
        //var t = (GameObject)m_parent.value;
        //m_instance.transform.SetParent((Transform)parent);
        m_instance.transform.localRotation = Quaternion.identity;
        m_instance.transform.localPosition = Vector3.zero;
        m_instance.transform.localScale = Vector3.one;
        m_instance.layer = 8;
        for(int i = 0; i < m_instance.transform.childCount; i++)
        {
            m_instance.transform.GetChild(i).gameObject.layer=8;
        }
        //SceneManager.MoveGameObjectToScene(m_instance, m_PreviewScene);
        Debug.Log(m_instance.name);
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (m_SceneCamera == null) return;
        m_SceneCamera.Render();
        if (m_PreviewTexture == null)
        {
            m_PreviewTexture = new Texture2D(m_size, m_size, TextureFormat.RGBAFloat, false);
        }
        RenderTexture.active = m_SceneCamera.targetTexture;
        m_PreviewTexture.ReadPixels(new Rect(0, 0, m_size, m_size), 0, 0, true);
        // m_PreviewTexture.ReadPixels(,);
        m_PreviewTexture.Apply();

        //RenderTexture.active = null;
    }
    //private void Export()
    //{
    //    m_SceneCamera.depthTextureMode = DepthTextureMode.Depth;

    //    m_SceneCamera.backgroundColor = new Color(0, 0, 0, 0);

    //    UpdateCamera();

    //    SaveTextureAsPNG(m_PreviewTexture, $"{m_instance.name}_Icon");

    //    m_SceneCamera.backgroundColor = Color.black;

    //    UpdateCamera();
    //}

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
    void Testing()
    {
        //Texture texture;
        MakeImage("Assets/Easy Icon Maker/", $"{m_instance.name}_Icon", 512);
    }
    public Texture2D MakeImage(string folderPathf, string name, int size)
    {
        //Light1.enabled = true;
        //Light2.enabled = true;

        if (folderPathf == "default")
            folderPathf = "Assets/Easy Icon Maker/";

        //if (name == "default" && PreviewObjectInstance != null)
        //    name = PreviewObjectInstance.name;
        //Debug.Log(m_SceneCamera.backgroundColor.ToString());
        //Debug.Log(m_SceneCamera.cameraType);
        m_SceneCamera.cameraType = CameraType.SceneView;
        string path = EditorUtility.SaveFilePanel(
         "Save Texture As PNG",
         "",
         $"{name}.png",
         "png"
     );
        Texture2D image = MakePreview(m_SceneCamera, path, name, size);

        m_SceneCamera.cameraType = CameraType.Game;
        AssetDatabase.Refresh();

        return image;
    }
//    private void Export()
//    {
//        int size = m_size;

//        // ---------- PASS 1 : COLOR ----------
//        RenderTexture colorRT = new RenderTexture(
//            size, size, 32, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.sRGB);
//        colorRT.Create();

//        m_SceneCamera.targetTexture = colorRT;
//        m_SceneCamera.clearFlags = CameraClearFlags.SolidColor;
//        m_SceneCamera.backgroundColor = Color.black;
//        m_SceneCamera.allowHDR = true;
//        m_SceneCamera.depthTextureMode =
//            DepthTextureMode.Depth | DepthTextureMode.DepthNormals;

//#if USING_URP
//    var camData = m_SceneCamera.GetUniversalAdditionalCameraData();
//    camData.renderPostProcessing = true;
//#endif

//        UpdateCamera();
//        m_SceneCamera.Render();

//        // ---------- PASS 2 : ALPHA ----------
//        RenderTexture alphaRT = new RenderTexture(
//            size, size, 32, RenderTextureFormat.R8, RenderTextureReadWrite.sRGB);
//        //alphaRT.s = false;
//        alphaRT.Create();

//        var originalMask = m_SceneCamera.cullingMask;
//        // IMPORTANT: put renderable objects on a dedicated layer
//        m_SceneCamera.cullingMask = 1 << 8;

//#if USING_URP
//    camData.renderPostProcessing = false;
//#endif

//        m_SceneCamera.targetTexture = alphaRT;
//        m_SceneCamera.backgroundColor = Color.black;

//        UpdateCamera();
//        m_SceneCamera.Render();

//        // Restore camera
//        m_SceneCamera.cullingMask = originalMask;
//        //m_SceneCamera.targetTexture = null;

//        // ---------- COMBINE ----------
//        Texture2D finalTex = CombineColorAndAlpha(colorRT, alphaRT);
//        FixAlphaBleeding(finalTex);
//        SaveTextureAsPNG(finalTex, $"{m_instance.name}_Icon");

//        // Cleanup
//        colorRT.Release();
//        alphaRT.Release();
//    }
//    private Texture2D CombineColorAndAlpha(
//    RenderTexture colorRT,
//    RenderTexture alphaRT)
//    {
//        int w = colorRT.width;
//        int h = colorRT.height;

//        Texture2D colorTex = new Texture2D(
//            w, h, TextureFormat.RGBAFloat, false);

//        RenderTexture.active = colorRT;
//        colorTex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
//        colorTex.Apply();

//        Texture2D alphaTex = new Texture2D(
//            w, h, TextureFormat.R8, false);

//        RenderTexture.active = alphaRT;
//        alphaTex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
//        alphaTex.Apply();

//        Color[] colorPixels = colorTex.GetPixels();
//        Color[] alphaPixels = alphaTex.GetPixels();

//        for (int i = 0; i < colorPixels.Length; i++)
//        {
//            colorPixels[i].a = alphaPixels[i].r;
//            //colorPixels[i].r = Mathf.LinearToGammaSpace(colorPixels[i].r);
//            //colorPixels[i].g = Mathf.LinearToGammaSpace(colorPixels[i].g);
//            //colorPixels[i].b = Mathf.LinearToGammaSpace(colorPixels[i].b);
//        }

//        colorTex.SetPixels(colorPixels);
//        colorTex.Apply();

//        RenderTexture.active = null;
//        DestroyImmediate(alphaTex);

//        return colorTex;
//    }
//    void FixAlphaBleeding(Texture2D tex)
//    {
//        Color[] pixels = tex.GetPixels();
//        int w = tex.width;
//        int h = tex.height;

//        for (int y = 1; y < h - 1; y++)
//            for (int x = 1; x < w - 1; x++)
//            {
//                int i = x + y * w;
//                if (pixels[i].a == 0f)
//                {
//                    Color sum = Color.clear;
//                    int count = 0;

//                    for (int oy = -1; oy <= 1; oy++)
//                        for (int ox = -1; ox <= 1; ox++)
//                        {
//                            int ni = (x + ox) + (y + oy) * w;
//                            if (pixels[ni].a > 0f)
//                            {
//                                sum += pixels[ni];
//                                count++;
//                            }
//                        }

//                    if (count > 0)
//                    {
//                        sum /= count;
//                        sum.a = 0f;
//                        pixels[i] = sum;
//                    }
//                }
//            }

//        tex.SetPixels(pixels);
//        tex.Apply();
//    }

    private static Texture2D GetTexture(Camera captureCamera, int size, Texture texBG)
    {
        Texture2D texture = new Texture2D(size, size);

        RenderTexture oldTexture = captureCamera.targetTexture;
        RenderTexture targetTexture = RenderTexture.GetTemporary(size, size, 32, RenderTextureFormat.ARGB32);

        if (texBG != null)
            Graphics.Blit(texBG, targetTexture);

        captureCamera.rect = new Rect(new Vector2(0, 0), new Vector2(512, 512));
        captureCamera.targetTexture = targetTexture;
        captureCamera.forceIntoRenderTexture = true;
        captureCamera.Render();
        Debug.Log(captureCamera.backgroundColor.ToString());
        RenderTexture tempActive = RenderTexture.active;
        RenderTexture.active = targetTexture;

        Rect rect = new Rect(0, 0, size, size);
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        RenderTexture.active = tempActive;

        captureCamera.targetTexture = oldTexture;
        RenderTexture.ReleaseTemporary(targetTexture);

        return texture;
    }
    public static Texture2D MakePreview(Camera captureCamera, string folderPathf, string name, int size = 256, Texture texBG = null)
    {
        Texture2D preview = GetTexture(captureCamera, size, texBG);
        if (folderPathf != null)
        {
            string iconPath = folderPathf;
            File.WriteAllBytes(iconPath, preview.EncodeToPNG());
            AssetDatabase.ImportAsset(iconPath, ImportAssetOptions.ForceUpdate);
        }

        return preview;
    }


}
