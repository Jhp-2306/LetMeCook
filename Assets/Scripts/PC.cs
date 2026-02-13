using Constants;
using UnityEngine;


public class PC : InteractiveBlock
{
    public string iD;
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
        Debug.Log("Calling the override" + gameObject.name);
        base.Init(EquipmentType.Furnitures,iD);
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
        return true;
    }
    public override void OnClick()
    {
        //power on the PC;
        HUDManagerDNDL.Instance.PcPowerOn();
    }
}
