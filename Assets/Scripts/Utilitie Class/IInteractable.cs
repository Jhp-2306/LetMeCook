using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public interface IInteractable
{
    public bool IsInteractable();
    EquipmentType GetEquipmentType();
    void OnClick();
    SO_EquipmentData GetEquipmentData();
   
}
