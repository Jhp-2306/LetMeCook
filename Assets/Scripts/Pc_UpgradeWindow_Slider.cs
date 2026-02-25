using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Pc_UpgradeWindow_Slider : MonoBehaviour
{
    public List<GameObject> LvlBars;
    public Image Icon;
    public TMPro.TextMeshProUGUI NameTxt;
    public TMPro.TextMeshProUGUI LvlTxt;

    private string id;

    public void SetUp(string name,Sprite icon,int level,string _id)
    {
        id = _id;
        NameTxt.text = name;
        Icon.sprite = icon;
        LvlTxt.text = $"Level {level}";
    }

}
