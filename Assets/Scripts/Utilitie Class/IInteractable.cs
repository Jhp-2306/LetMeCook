using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public interface IInteractable
{
    public bool IsInteractable();
    EquipmentType GetEquipmentType();
    void OnClick(bool ishandfull=false);
    SO_EquipmentData GetEquipmentData();
   
}
