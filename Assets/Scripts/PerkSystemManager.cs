using Constants;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class PerkSystemManager : Singletonref<PerkSystemManager>
{

    [Space(10)]
    [Header("Perk System")]
    [SerializeField]
    private SO_PerkSystem perkData;
    private Dictionary<perkSystem_Value, int> CurrentPerkValues;// perkname and Rank

    public void init()
    {
        //Base Value
        if (CurrentPerkValues == null)
        {
            CurrentPerkValues = new Dictionary<perkSystem_Value, int>();
            AddorUpdateValue(perkSystem_Value.EquipmentOwnePrice, 0);//Base value 
            AddorUpdateValue(perkSystem_Value.IncomeTaxRate, 0);//Base value 
            AddorUpdateValue(perkSystem_Value.Instant, -1);//Base value 
            AddorUpdateValue(perkSystem_Value.BlackMarket, -1);//Base value 
            AddorUpdateValue(perkSystem_Value.Auto, -1);//Base value 
            AddorUpdateValue(perkSystem_Value.OrderWaitingPeroid, -1);//Base value 
            AddorUpdateValue(perkSystem_Value.Govt_Tax, 0);//Base value 
        }
        Debug.Log(perkData.perkData.Count);
    }

    void AddorUpdateValue(perkSystem_Value val, int rank)
    {
        if (CurrentPerkValues.ContainsKey(val))
        {
            CurrentPerkValues[val] = /*GetPerkValueFromTheList(perkSystem_Value.EquipmentOwnePrice, rank)*/rank;
        }
        else
        {
            CurrentPerkValues.Add(val, /*GetPerkValueFromTheList(val, rank)*/rank);
        }
    }

    #region Getter
    public int GetCurrentRankforValue(perkSystem_Value value) => CurrentPerkValues[value];
    public string GetdesForValue(perkSystem_Value value, int rank) => GetPerk(value, rank).Des;
    public string GetPerkNameForValue(perkSystem_Value value, int rank) => GetPerk(value, rank).PerkName;
    public int GetPerkValue(perkSystem_Value val)
    {
        return GetPerkValueFromTheList(val,CurrentPerkValues[val]);
    }
    int GetPerkValueFromTheList(perkSystem_Value val, int rank)
    {
        return GetPerk(val, rank).Value;
    }


    PerkData GetPerk(perkSystem_Value val, int rank)
    {
        var temp = perkData.perkData;
        foreach (var t in temp)
        {
            if (t.Type == val)
            {
                if (t.Rank == rank)
                { return t; }
            }
        }
        return new PerkData();
    }
    #endregion
    public void UpgradePerk(perkSystem_Value val, int rank)
    {
        AddorUpdateValue(val, rank);
    }

}
