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
    }
    public override void Init()
    {
        savedata = new SaveDataTemplate();
        savedata.id = GameSaveDNDL.GenerateId(EquipmentType.Refrigerator.ToString());
        savedata.Position = transform.position;
        savedata.Rotation = transform.rotation;
        savedata.Type = EquipmentType.Refrigerator;
        //if (storageSystem == null)
        //{
        //    storageSystem = new BasicStorageSystem<IStorageItem>(Slots);
        //}
        //savedata.Data = storageSystem.GetAllDataInString();
        Data = this.GetEquipmentData();
        equipmentType = EquipmentType.Refrigerator;
        //BeforeSaving();
       //GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
       //GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
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
