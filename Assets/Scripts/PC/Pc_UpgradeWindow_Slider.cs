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
    public Button upgradeBtn;

    private string id;
    private int _lvl;
    public void SetUp(string name,int level,string _id/*,Sprite icon*/)
    {
        id = _id;
        NameTxt.text = name;
       // Icon.sprite = icon;
       _lvl= level;
        LvlTxt.text = $"Level {_lvl+1}";
        upgradeBtn.onClick.RemoveAllListeners();
        upgradeBtn.onClick.AddListener(delegate () {
            _lvl++;
            LvlTxt.text = $"Level {_lvl + 1}";
            GameDataDNDL.Instance.LevelUpCallback(id,_lvl); });
    }

    public void onClick()
    {

    }

}
