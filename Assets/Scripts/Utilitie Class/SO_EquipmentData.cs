using System.Collections;
using UnityEngine;
using Constants;
using System;

[CreateAssetMenu(fileName ="TableEquipmentData",menuName = "ScriptableObject/EquipmentData")]
public class SO_EquipmentData : ScriptableObject
{
    public string DisplayFuntionName;
    public string HandFullDisplayFuntionName;
    public EquipmentType Type;
    public GameObject Functionatily;

    
}
[Serializable]
public class equipmentData
{
    public string DisplayFuntionName;
    public EquipmentType Type;
    public string name;
    public int price;
    public GameObject Prefab;
}
