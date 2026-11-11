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
    [SerializeField]
    private float HoldDurationbase = 4.5f;
    private float lvlUpgradeDurationPercentage;
    public override bool IsInteractionSatisfied()
    {
        //return base.IsInteractionSatisfied();
        //var player = GameDataDNDL.Instance.GetPlayer();
        if (player.isHandsfull && (player.InHand.IGetType() == typeofhandheld.plate || player.InHand.IGetType() == typeofhandheld.ingredients)) return true;
        if (!player.isHandsfull && !IsEmpty) return true;
        return false;
    }

    public override bool IsHoldInteractionSatisfied()
    {
        if (!player.isHandsfull&&!IsEmpty&& myPlate.canCombine())return true;
        return false;
    }
    public override float GetOverrideHoldDuration()
    {
       return HoldDurationbase-(HoldDurationbase* lvlUpgradeDurationPercentage);
    }
    public override bool IsOverrideHoldInteraction()
    {
        return true;
    }
    public override void OnClick()
    {
        var player = GameDataDNDL.Instance.GetPlayer();
        //if (IsEmpty)
        if (player.isHandsfull)
        {
            if (player.InHand.IGetType() == typeofhandheld.plate && IsEmpty)
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
                    var go = ObjectPoolingManager.Instance.GetaPool(ObjectPoolingManager.PrefabTags.Plate);
                    //var go = Instantiate(AssetLoader.Instance.PlatesPrefab);
                    go.transform.SetParent(this.transform);
                    go.transform.localPosition = PlacementPositions.localPosition;
                    myPlate = go.GetComponent<Plate>();
                    myPlate.gameObject.SetActive(true);
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
                    player.RemoveFromHand(ObjectPoolingManager.PrefabTags.Ingredients);
                    //player.MovetheHandheld(PlacementPositions);
                }
            }
            else
            {
                //Swap the plates
            }
        }
        //else
        else if (!player.isHandsfull)//Pick Up the plate
        {
            //pick up the dish from the table
            player.PickSomeThing(myPlate, myPlate.gameObject);
            myPlate = null;
        }

    }

    public override void OnHold()
    {
        //Combine
        myPlate.MergeTheDish();
        myPlate.MergerVFX.SetActive(false);
    }
    public override void OnHoldStart()
    {
        myPlate.MergerVFX.SetActive(true);
    }
    public override void OnHoldFailed()
    {
        myPlate.MergerVFX.SetActive(false);
    }
    public override void LoadLevelUpgrades()
    {
        var temp = Data.upgradeData[savedata.level];
        lvlUpgradeDurationPercentage = temp.GetUpgradeValue(E_ValueName.Speed) / 100;
    }
}
