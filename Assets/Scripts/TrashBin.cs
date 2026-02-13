using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : InteractiveBlock
{
    #region Save Mechanics
    public override void ReadFromSave(SaveDataTemplate _data)
    {
        base.ReadFromSave(_data);
        ///Any additional init/data distribution code here

        ///in-between here
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
    }
    public override void Init(EquipmentType _equip = EquipmentType.none, string item = "")
    {
        base.Init(EquipmentType.TrashBin);
        ///Any additional init/data distribution code here

        ///in-between here
        BeforeSaving();
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
    }
    void BeforeSaving()
    {
        GameSaveDNDL.Instance.AddSaveData(savedata);
    }
    #endregion
    public override bool IsInteractionSatisfied()
    {
        if(GameDataDNDL.Instance.GetPlayer().isHandsfull) return true; return false;
    }
    // this Will Remove anything which play has in his hands
    public override void OnClick()
    {
        if (GameDataDNDL.Instance.GetPlayer().isHandsfull)
        {
            GameDataDNDL.Instance.GetPlayer().RemoveFromHand(); 
        }else
        {
            HUDManagerDNDL.Instance.ShowToastMsg("you hava nothing to throw in trash");
        }
    }
}
