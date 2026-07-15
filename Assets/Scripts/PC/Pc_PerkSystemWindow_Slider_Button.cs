using Constants;
using UnityEngine;
using UnityEngine.UI;

public class Pc_PerkSystemWindow_Slider_Button : MonoBehaviour
{
    public perkSystem_Value Value;
    public int Rank;
    
    
    public bool isNeededUnlock()=> PerkSystemManager.Instance.GetCurrentRankforValue(Value)+1==Rank /*&& PerkSystemManager.Instance.GetCurrentRankforValue(Value)<= Rank*/;
    public bool isUnlocked()=> /*PerkSystemManager.Instance.GetCurrentRankforValue(Value)+1==Rank /*&&*/ PerkSystemManager.Instance.GetCurrentRankforValue(Value)<= Rank;
    private void Awake()
    {
        Pc_PerkSystemWindow.Oninit -= init;
        Pc_PerkSystemWindow.Oninit += init;
    }
    public void init()
    {
        //TODO: Set unlock/Lock Assets here
        if (isUnlocked())
        {

        }
        else
        {

        }
    }
}
