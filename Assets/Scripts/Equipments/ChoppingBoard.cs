using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ChoppingBoard : MonoBehaviour, IInteractable
{
    public EquipmentType Type;
    public EquipmentType GetEquipmentType() => Type;
    public SO_EquipmentData myEquipmentData;

    public void OnClick()
    {
        CustomLogs.CC_Log("Chopping", "red");
    }

    public SO_EquipmentData GetEquipmentData()
    {
        return myEquipmentData;
    }
}
