using Constants;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


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

    public List<IngredientType> AccessableIngredient;
    private float width=0.05f;
    private float lvlUpgradeWidthPercentage = 0;

    
    public override void OnClick()
    {
        CustomLogs.CC_Log("Chopping", "red");
        var _player = GameDataDNDL.Instance.GetPlayer();
        if (HUDManagerDNDL.Instance.MiniGame.isMiniGameRunning)
        {
            HUDManagerDNDL.Instance.MiniGame.onClick();
            HUDManagerDNDL.Instance.HideinteractionButton();
            return;
        }
        if ((Ingredient)_player.InHand != null)
        {
            //Remove from hand
            InputManager.Instance.DisablePlayerMovement(true);
            CurrentIngredientAdded = ((Ingredient)_player.InHand).ingrendient;
            _player.RemoveFromHand();
            if (CheckPerkActivation() && m_isPerkActive)
            {
                if (m_remainingChops > 0)
                {
                    OnMiniGameDone(m_masterChops);
                    InputManager.Instance.DisablePlayerMovement(false);
                    m_remainingChops--;
                    return;
                }
            }
            Debug.Log(CustomLogs.CC_Log("Starting Mini Game", "cyan"));
            //Mini Game
            HUDManagerDNDL.Instance.MiniGame.FillupMiniGameStart(0.6f, width+(lvlUpgradeWidthPercentage*width));
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
        //var go = Instantiate(AssetLoader.Instance.itemPrefab);
        var go = ObjectPoolingManager.Instance.GetaPool(ObjectPoolingManager.PrefabTags.Ingredients);
        var details = AssetLoader.Instance.GetIngredientSO(CurrentIngredientAdded.Ingredient, CurrentIngredientAdded.processed);
        go.GetComponent<Ingredient>().Setup(details.mesh, details.type, details.handheldtype, details.material, details.process);
        GameDataDNDL.Instance.GetPlayer().PickSomeThing(go.GetComponent<IHandHeld>(), go);
        if (CheckPerkActivation())
        {
            if (!m_isPerkActive)
            {
                m_isPerkActive = true;
                m_remainingChops = 5;
                if(!PerkSystemManager.Instance.CheckPerkUnlocked(perkSystem_Value.Instant, 1))
                m_masterChops = result;
                else
                    m_masterChops = 1;
                

            }
            else
            if (m_remainingChops <= 0 && m_isPerkActive)
            {
                m_isPerkActive = false;
                //m_remainingChops = 5;
                m_masterChops = -1;
            }
        }
        HUDManagerDNDL.Instance.MiniGame.OnMiniGameOver -= OnMiniGameDone;
    }

    public override bool IsInteractionSatisfied()
    {
        var _player = GameDataDNDL.Instance.GetPlayer();
        if (!HUDManagerDNDL.Instance.MiniGame.isMiniGameRunning && (_player.isHandsfull == false || _player.InHand.GetGameObject().GetComponent<Ingredient>().ingrendient.processed != ProcessStatus.None)) return false;
        if (HUDManagerDNDL.Instance.MiniGame.isMiniGameRunning || (_player.InHand.IGetType() == typeofhandheld.ingredients &&
            AccessableIngredient.Contains(_player.InHand.GetGameObject().GetComponent<Ingredient>().ingrendient.Ingredient))) return true;
        return false;
        //return 
    }
    #endregion
    public override void LoadLevelUpgrades()
    {
        var temp = Data.upgradeData[savedata.level];
       lvlUpgradeWidthPercentage=temp.GetUpgradeValue(E_ValueName.Size)/100;
    }
    #region Perk
    private bool m_isPerkActive;
    private int m_remainingChops;
    private int m_masterChops = -1;

    public bool CheckPerkActivation()
    {
        return PerkSystemManager.Instance.CheckPerkUnlocked(perkSystem_Value.Instant, 0);

    }

    #endregion
}
