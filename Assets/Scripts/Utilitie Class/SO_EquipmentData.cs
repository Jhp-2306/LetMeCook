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
}
[Serializable]
public struct UpgradeData
{
    public E_ValueName ValueName;
    public float ValueInPercentage;
}
[Serializable]
public enum E_ValueName
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
