using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : InteractiveBlock
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
        base.Init(EquipmentType.Table);
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
    //bool isempty;

    public Plate myPlate;
    // public GameObject PlateVisuals;
    public Transform PlacementPositions;
    public bool IsEmpty => myPlate == null;

    public override bool IsInteractionSatisfied()
    {
        //return base.IsInteractionSatisfied();
        var player = GameDataDNDL.Instance.GetPlayer();
        if (player.isHandsfull && (player.InHand.IGetType() == typeofhandheld.plate || player.InHand.IGetType() == typeofhandheld.ingredients)) return true;
        if (!player.isHandsfull && !IsEmpty) return true;
        return false;
    }
    public override void OnClick()
    {
        var player = GameDataDNDL.Instance.GetPlayer();
        //if (IsEmpty)

        if (player.isHandsfull)
        {
            if (player.InHand.IGetType() == typeofhandheld.plate)
            {
                //myPlate.DestroyMe();
                myPlate = player.InHand.GetGameObject().GetComponent<Plate>();
                player.MovetheHandheld(PlacementPositions);
            }
            else
            if (player.InHand.IGetType() == typeofhandheld.ingredients)
            {
                // if there is no plate then we got to create a new one
                if (myPlate == null)
                {
                    var go = Instantiate(AssetLoader.Instance.PlatesPrefab);
                    go.transform.SetParent(this.transform);
                    go.transform.localPosition = PlacementPositions.localPosition;
                    myPlate = go.GetComponent<Plate>();
                }
                // here we need to check is it possible to merge the we can add it here else it will there
                if (!myPlate.tryMergeRecipes(player.InHand.GetGameObject().GetComponent<Ingredient>().ingrendient))
                {
                    //Try swap or toast msg
                    HUDManagerDNDL.Instance.ShowToastMsg("Cant be merged");
                }
                else
                {
                    myPlate.gameObject.SetActive(true);
                    player.MovetheHandheld(PlacementPositions);
                }
            }
        }

        //else

        else if (!player.isHandsfull)
        {
            //pick up the dish from the table
            player.PickSomeThing(myPlate, myPlate.gameObject);
            myPlate = null;
        }

    }


}
