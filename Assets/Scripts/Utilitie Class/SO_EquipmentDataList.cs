using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentDataList", menuName = "ScriptableObject/EquipmentListData")]
public class SO_EquipmentDataList : ScriptableObject
{
    public List<equipmentData> equipmentDataList;
}
