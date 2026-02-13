using Constants;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlateHolder : InteractiveBlock
{
    //int maxplates = 4;
    //int totalplateinarack = 4;
    public override void ReadFromSave(SaveDataTemplate _data)
    {
        base.ReadFromSave(_data);
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
    }
    public override void Init(EquipmentType _equip = EquipmentType.none, string item = "")
    {
        base.Init(EquipmentType.PlateTray, item);
        //savedata = new SaveDataTemplate();
        //savedata.id = GameSaveDNDL.GenerateId(EquipmentType.PlateTray.ToString());
        //savedata.Position = transform.position;
        //savedata.Rotation = transform.rotation;
        //savedata.Type = EquipmentType.PlateTray;
        //Data = this.GetEquipmentData();
        equipmentType = EquipmentType.PlateTray;
        BeforeSaving();
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
    }
    void BeforeSaving()
    {
        GameSaveDNDL.Instance.AddSaveData(savedata);
    }
    public override void OnClick()
    {
        //var player = ;
        if (GameDataDNDL.Instance.GetPlayer().isPlayerHandEmpty)
        {
            //if (canRemovePlate())
            {
            //Instantiate plate in  players hands
            var go = Instantiate(AssetLoader.Instance.PlatesPrefab);
            GameDataDNDL.Instance.GetPlayer().PickSomeThing(go.GetComponent<IHandHeld>(), go);
            }
            //else
            //{
            //    Debug.Log(CustomLogs.CC_TagLog("Plate Holder", "no plates"));
            //}

        }
        else
        {
            if(GameDataDNDL.Instance.GetPlayer().InHand.IGetType()== typeofhandheld.plate)
            {
                //the check if it only a plate then keep it back
                if(GameDataDNDL.Instance.GetPlayer().InHand.GetGameObject().GetComponent<Plate>().isPlateEmpty)
                {
                    //add the plate back and
                    //remove it from the hand
                    GameDataDNDL.Instance.GetPlayer().RemoveFromHand();
                }
            }
            else
            {
                Debug.Log(CustomLogs.CC_TagLog("Plate Holder", "players Hand's are full"));
            }
        }
    }
    public override bool IsInteractionSatisfied()
    {
        return true;
    }
    //bool canRemovePlate()
    //{
    //    if (totalplateinarack > 0)
    //    {
    //        totalplateinarack--;
    //        return true;
    //    }
    //    else
    //        return false;
    //}
    //bool canAddPlate()
    //{
    //    if (totalplateinarack <maxplates)
    //    {
    //        totalplateinarack++;
    //        return true;
    //    }
    //    else
    //        return false;
    //}
}
