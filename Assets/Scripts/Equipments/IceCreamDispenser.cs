using Constants;
using UnityEngine;

public class IceCreamDispenser : InteractiveBlock
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
    public override void Init(EquipmentType _equip = EquipmentType.none, string _prefabid = "")
    {
        base.Init(EquipmentType.IceCreamDispenser, _prefabid);
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

    int TotalIceCreamLoadSlot= 5;
    int CurrentIceCreamLoad = 0;
    public override void OnClick()
    {
        //Generate IceCream
    }
}
