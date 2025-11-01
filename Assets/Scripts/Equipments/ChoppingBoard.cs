using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ChoppingBoard : InteractiveBlock
{

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
    public void OnMiniGameDone(int result)
    {
        //Pick up
        Debug.Log(CustomLogs.CC_Log($"Ending {result}", "cyan"));
        CurrentIngredientAdded.processed = result == 0 ? processIngredient.Chopped : processIngredient.FineChopped;
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
}
