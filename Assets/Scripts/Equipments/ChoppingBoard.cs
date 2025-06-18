using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ChoppingBoard : MonoBehaviour, IInteractable
{
    public EquipmentType Type;
    public EquipmentType GetEquipmentType() => Type;
    public SO_EquipmentData myEquipmentData;

    public Ingredient.Str_Ingredient_Data CurrentIngredientAdded;

    public void OnClick(bool ishandfull = false)
    {
        CustomLogs.CC_Log("Chopping", "red");
        var _player=GameDataDNDL.Instance.GetPlayer();
        if ((Ingredient)_player.InHand != null)
        {
            //Remove from hand
            CurrentIngredientAdded=((Ingredient)_player.InHand).ingrendient;
            _player.RemoveFromHand();
            CustomLogs.CC_Log("Starting Mini Game", "cyan");
            //Mini Game

            //Pick up
        }
    }

    public SO_EquipmentData GetEquipmentData()
    {
        return myEquipmentData;
    }

    public bool IsInteractable()
    {
        if (!GameDataDNDL.Instance.GetPlayer().isPlayerHandEmpty)
        {
            if ((GameDataDNDL.Instance.GetPlayer().InHand.IGetType() != typeofhandheld.box || !GameDataDNDL.Instance.GetPlayer().isHandsfull))
                return true;
        }
        else
            return true;
        return false;
    }
}
