using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ChoppingBoard : InteractiveBlock
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
        base.Init(EquipmentType.Cutting_Board, _prefabid);
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

    #region Main Mechanics
    public ProcedureStep CurrentIngredientAdded;
   
    public override void OnClick()
    {
        CustomLogs.CC_Log("Chopping", "red");
        var _player=GameDataDNDL.Instance.GetPlayer();
        if (HUDManagerDNDL.Instance.MiniGame.isMiniGameRunning)
        {
            HUDManagerDNDL.Instance.MiniGame.onClick();
            return;
        }
        if ((Ingredient)_player.InHand != null)
        {
            //Remove from hand
            CurrentIngredientAdded=((Ingredient)_player.InHand).ingrendient;
            _player.RemoveFromHand();
            Debug.Log(CustomLogs.CC_Log("Starting Mini Game", "cyan"));
            //InputManager.Instance.OnHoldUI();
            //Mini Game
            HUDManagerDNDL.Instance.MiniGame.FillupMiniGameStart(0.6f,0.05f);
            HUDManagerDNDL.Instance.MiniGame.OnMiniGameOver -= OnMiniGameDone;
            HUDManagerDNDL.Instance.MiniGame.OnMiniGameOver += OnMiniGameDone;
            return;
        }      

    }
    public void init_FTUT()
    {
        var _player = GameDataDNDL.Instance.GetPlayer();
        if ((Ingredient)_player.InHand != null)
        {
            //Remove from hand
            CurrentIngredientAdded = ((Ingredient)_player.InHand).ingrendient;
            _player.RemoveFromHand();
            Debug.Log(CustomLogs.CC_Log("Starting Mini Game", "cyan"));
            //InputManager.Instance.OnHoldUI();
            //Mini Game
            HUDManagerDNDL.Instance.MiniGame.FTUT(0.6f, 0.05f);
            HUDManagerDNDL.Instance.MiniGame.OnMiniGameOver -= OnMiniGameDone;
            HUDManagerDNDL.Instance.MiniGame.OnMiniGameOver += OnMiniGameDone;
            return;
        }
    }
    public void OnMiniGameDone(int result)
    {
        //Pick up
        Debug.Log(CustomLogs.CC_Log($"Ending {result}", "cyan"));
        CurrentIngredientAdded.processed = result == 0 ? ProcessStatus.Chopped : ProcessStatus.FineChopped;
        var go = Instantiate(AssetLoader.Instance.itemPrefab);
        var details = AssetLoader.Instance.GetIngredientSO(CurrentIngredientAdded.Ingredient, CurrentIngredientAdded.processed);
        go.GetComponent<Ingredient>().Setup(details.mesh, details.type, details.handheldtype, details.material, details.process);
        GameDataDNDL.Instance.GetPlayer().PickSomeThing(go.GetComponent<IHandHeld>(), go);
        HUDManagerDNDL.Instance.MiniGame.OnMiniGameOver -= OnMiniGameDone;
    }
   

    public override bool IsInteractionSatisfied()
    {
        var player = GameDataDNDL.Instance.GetPlayer();
        if (!player.isPlayerHandEmpty)
        {
            if ((player.InHand.IGetType() != typeofhandheld.box || !player.isHandsfull))
                return true;
        }
        else
            return true;
        return false;
    }
    #endregion
}
