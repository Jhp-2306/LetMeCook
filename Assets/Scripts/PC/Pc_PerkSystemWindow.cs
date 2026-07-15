using System;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;


public class Pc_PerkSystemWindow : MonoBehaviour
{
    //TT=ToolTip
    public static Action Oninit = delegate { };
    public GameObject TooltipParent;
    public Image TT_Icon;
    public TMPro.TextMeshProUGUI TT_Name;
    public TMPro.TextMeshProUGUI TT_Des;
    public GameObject TT_BuyButtton;
    public TMPro.TextMeshProUGUI TT_BuyButttontxt;
    private int m_Price;
    private Pc_PerkSystemWindow_Slider_Button m_value;
    public void init()
    {
        Oninit();
        m_value=new Pc_PerkSystemWindow_Slider_Button();
    }
    public void OnClickOnPerk(Pc_PerkSystemWindow_Slider_Button value)
    {
        m_value = value;
        TooltipParent.SetActive(true);
        TT_Name.text = PerkSystemManager.Instance.GetPerkNameForValue(value.Value, value.Rank);
        TT_Des.text = PerkSystemManager.Instance.GetdesForValue(value.Value, value.Rank);
        //Debug.Log($"isUnlockd={value.isUnlocked()},Perkrank{PerkSystemManager.Instance.GetCurrentRankforValue(value.Value)},my rank{value.Rank}");
        TT_BuyButtton.SetActive(value.isNeededUnlock());
        TT_BuyButttontxt.text=m_Price.ToString();
    }

    public void PurchasePerk()
    {
        if (GameDataDNDL.Instance.tryDeductingCurrency(m_Price))
        {
            //Activate the perk
            PerkSystemManager.Instance.UpgradePerk(m_value.Value, m_value.Rank);
            CloseToolTip();
            //play the animations
        }
        else
        {
            //Fail to Activate the perk
        }
    }
    
    public void CloseToolTip()
    {
        TooltipParent.SetActive(false);
        m_value = null;
    }
}
