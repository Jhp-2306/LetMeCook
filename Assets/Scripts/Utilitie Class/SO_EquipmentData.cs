using System.Collections;
using UnityEngine;
using Constants;
using System;
using NUnit.Framework;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="TableEquipmentData",menuName = "ScriptableObject/EquipmentData")]
public class SO_EquipmentData : ScriptableObject
{
    public string DisplayFuntionName;
    public string HoldFuntionName;
    public string HandFullDisplayFuntionName;
    public EquipmentType Type;
    public List<LevelUpgradeData> upgradeData;

    public string IconID;
    public string LevelupgradeDes;
    public List<string> GetUpgradeDes()
    {
        List<string> ret = new List<string>();
        foreach (LevelUpgradeData upgradeData in upgradeData)
        {
            ret.Add(upgradeData.GetUpgradeDes());
        }
        return ret;
    }
}
[Serializable]
public class LevelUpgradeData
{
    public List<UpgradeData> Upgrades;
    public float GetUpgradeValue(E_ValueName upgradeData)
    {
        foreach (var upgrade in Upgrades) { 
        if(upgrade.ValueName.Equals(upgradeData))return upgrade.ValueInPercentage;
        }
        return -1f;
    }
    public string GetUpgradeDes()
    {
        string val="";
        foreach (var upgrade in Upgrades)
        {
            val = $"{string.Format(upgrade.ValueDes, upgrade.ValueInPercentage)}\n";
        }
        return val;
    }

}
[Serializable]
public struct UpgradeData// Equipment Upgrade Save Data
{
    public string ValueDes;
    public E_ValueName ValueName;
    public float ValueInPercentage;
}
[Serializable]
public enum E_ValueName//Equipment Upgrade Values 
{
    BillRate,
    Speed,
    Assite,
    Slots,
    Size,
    none,
}
[Serializable]
public class equipmentData
{
    public string DisplayFuntionName;
    public EquipmentType Type;
    public string name;
    public int price;
    public string Prefab;
    public string Icon;
}
