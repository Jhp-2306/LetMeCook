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
    public void SetUp(string name, int level, string _id, string icon, List<string> Des, int maxlevel)
    {
        id = _id;
        _lvl = level;
        NameTxt.text = string.Format(name, _lvl == maxlevel - 1 ? "<color=red>Max</color>" : _lvl + 1);
        Icon.sprite = AssetLoader.Instance.GetIcons(icon);
        LvlTxt.text = _lvl == maxlevel - 1 ? "<color=red>Max</color>" : Des[_lvl+1];
        upgradeBtn.onClick.RemoveAllListeners();
        upgradeBtn.onClick.AddListener(delegate ()
        {
            if (_lvl < maxlevel - 1)
            {
                _lvl++;
                NameTxt.text = string.Format(name, _lvl == maxlevel - 1 ? "<color=red>Max</color>" : _lvl + 1);
                LvlTxt.text = _lvl == maxlevel - 1 ? "<color=red>Max</color>" : Des[_lvl + 1];
                GameDataDNDL.Instance.LevelUpCallback(id, _lvl);
            }

        });
    }

    public void onClick()
    {

    }

}
