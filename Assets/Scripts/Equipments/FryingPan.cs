using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FryingPan : InteractiveBlock
{
    //4 Slots of ingredient
    //start to cook for timer
    public ProcedureStep Slots;
    private bool isSlotFilled;
    //private const int MaxSlots = 0;
    private float defaultCookingTime = 30f;
    private float OverCookaddOnTimer = 20f;
    private float OverCookBeforeAddOn = 30f;
    private float undercookPercentage = 0.08f;
    //Hud
    public Image Progress;
    public Image RedZone;
    public Image GreenZone;
    public Image BlueZone;
    public Image SlotsIcon;
    public GameObject HUD, ProgressbarHUD, SlotsHUD;
    ProcessStatus dishStatus;
    bool isCooking;
    bool isPicked;
    private bool isInteractable;
    Coroutine CookingCoroutine;
    private float lvlUpgradeDurationPercentage;
    private float CurrentcookingdishPriceMulti;
    public Sprite itemHolder_grayScale;
    public GameObject NormalVFX;
    public GameObject BurnedVFX;
    private Dishes Currentcookingdish;
    private float CurrentcookingdishPrice;
    [SerializeField]
    private List<IngredientType> AccessibleIngredients;


    #region Save Mechanics
    public override void ReadFromSave(SaveDataTemplate _data)
    {
        base.ReadFromSave(_data);
        ///Any additional init/data distribution code here
        //Slots = /*new List<ProcedureStep>();*/
        Slots = new ProcedureStep();
        HUD.SetActive(false);
        isPicked = true;
        isCooking = false;
        isInteractable = true;
        ///in-between here
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
    }
    void BeforeSaving()
    {
        GameSaveDNDL.Instance.AddSaveData(savedata);
    }
    #endregion
    public override void Init(EquipmentType _equip = EquipmentType.none, string item = "")
    {
        base.Init(EquipmentType.Frying_pan, item);
        ///{Any additional init/data distribution code here
        Slots = new ProcedureStep();
        isSlotFilled = false;
        HUD.SetActive(false);
        isPicked = true;
        isCooking = false;
        isInteractable = true;
        ///in-between here}
        BeforeSaving();
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
    }
    public void AddIngredient(Ingredient ingredient)
    {
        if (!isSlotFilled)
        {
            //Slots.Add(ingredient.ingrendient);
            Slots = ingredient.ingrendient;
            //After Adding Ingredient inHandItem Removeds
            GameDataDNDL.Instance.GetPlayer().RemoveFromHand();
            isSlotFilled = true;
            SlotsUpdate();
        }
        else
        {
            Debug.Log(CustomLogs.CC_TagLog("Frying Pan", "Failed To Add Ingredient-- Max Slots Filled"));
        }
    }
    public void SlotsUpdate()
    {
        HUD.SetActive(true);
        SlotsHUD.SetActive(true);
        ProgressbarHUD.SetActive(false);
        // int idx = 0;
        if (isSlotFilled)
        {
            SlotsIcon.sprite = AssetLoader.Instance.GetIngredientIcon(Slots.Ingredient, Slots.processed);
            //idx++;
        }
    }
    public override bool IsInteractionSatisfied()
    {
        //return isInteractable;
        var player = GameDataDNDL.Instance.GetPlayer();
        return (player.isHandsfull && ((player.InHand.IGetType() == typeofhandheld.plate && ((isCooking && !isPicked) || (!isCooking && !isPicked))) ||
            (player.InHand.IGetType() == typeofhandheld.ingredients && isPicked && AccessibleIngredients.Contains(player.InHand.GetGameObject().GetComponent<Ingredient>().ingrendient.Ingredient))));//Basic Condition to Add 
                                                                                                                                                                                                      //||(!player.isHandsfull && Slots.Count > 1);//for start Cooking;

    }
    public override bool IsHoldInteractionSatisfied()
    {
        return !isCooking && isPicked && isSlotFilled;
    }
    public override void OnClick()
    {
        //Picking Mechanics
        if (isCooking && !isPicked)
        {
            //Stop the coroutine there and pick the dish
            stopFryer();
            isPicked = tryPickupDish();

            Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"Done Cooking Dish-- status{Slots.processed}"));
        }
        if (!isCooking && !isPicked)
        {
            isPicked = tryPickupDish();
            StopCoroutine(CookingCoroutine);

            Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"Done Cooking Dish-- status{dishStatus}"));
        }
        //ADD Mechanics
        var player = GameDataDNDL.Instance.GetPlayer();
        if (player.isHandsfull && player.InHand.IGetType() == typeofhandheld.ingredients && isPicked)
        {
            AddIngredient(player.InHand.GetGameObject().GetComponent<Ingredient>());
        }
        else
        {
            Debug.Log(CustomLogs.CC_TagLog("Frying Pan", "Failed To Add Ingredient-- Player Dont Have Any ingredient to add"));
        }

    }

    public override void OnHold()
    {
        //base.OnHold();
        if (isCooking || !isPicked) return;
        if (isSlotFilled)
        {
            Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"Start Frying,{defaultCookingTime - (defaultCookingTime * lvlUpgradeDurationPercentage)}"));
            CookingCoroutine = StartCoroutine(StartCooking(defaultCookingTime - (defaultCookingTime * lvlUpgradeDurationPercentage)));
        }
        else
        {
            Debug.Log(CustomLogs.CC_TagLog("Frying Pan", "Not Enough Ingredient Added"));
        }
    }
    bool tryPickupDish()
    {
        if (Slots.Ingredient != IngredientType.Steak)
            Currentcookingdish = GameDataDNDL.Instance.GetFryerDish(Slots);
        else
        {
            if (dishStatus == ProcessStatus.Cooked)
                Currentcookingdish = Dishes.SteakMid;
            if (dishStatus == ProcessStatus.UnderCooked)
                Currentcookingdish = Dishes.SteakRare;
            if (dishStatus == ProcessStatus.OverCooked)
                Currentcookingdish = Dishes.SteakWell;
        }
        CurrentcookingdishPrice = GameDataDNDL.Instance.GetFryerDishPrice(Currentcookingdish);
        Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"dish{Currentcookingdish},{CurrentcookingdishPrice}"));
        var player = GameDataDNDL.Instance.GetPlayer();
        if (player.InHand != null && player.InHand.GetGameObject().GetComponent<Plate>() != null)
        {
            if (player.InHand.GetGameObject().GetComponent<Plate>().isPlateEmpty)
            {
                if (Slots.Ingredient != IngredientType.Steak)
                    GetFoodcookMulti();
                if(Currentcookingdish!=Dishes.trash)
                player.InHand.GetGameObject().GetComponent<Plate>().pickupDish(Currentcookingdish, CurrentcookingdishPrice, CurrentcookingdishPriceMulti);
                if(Currentcookingdish == Dishes.trash)
                {
                    Slots.processed = dishStatus!=ProcessStatus.Cooked?dishStatus:ProcessStatus.Fryed;
                    player.InHand.GetGameObject().GetComponent<Plate>().tryMergeRecipes(Slots);
                }
                //Refresh the Slots
                ResetStove();
                return true;
            }
            //throw a toast here or try to merge the dish?
            HUDManagerDNDL.Instance.ShowToastMsg("plate's not empty");
            return false;
        }
        else// player without plate
        {
            HUDManagerDNDL.Instance.ShowToastMsg("you need a plate");
            Debug.Log(CustomLogs.CC_TagLog("Stove", "issue with the player hands"));
            return false;
        }
    }
    void GetFoodcookMulti()
    {
        switch (dishStatus)
        {
            case ProcessStatus.None: break;
            case ProcessStatus.UnderCooked:
                CurrentcookingdishPriceMulti += 0.75f; break;
            case ProcessStatus.Cooked:
                CurrentcookingdishPriceMulti += 1.5f; break;
            case ProcessStatus.OverCooked:
                CurrentcookingdishPriceMulti += 1f; break;
            case ProcessStatus.Burned:
                CurrentcookingdishPriceMulti = 1; break;
        }
    }
    public void ResetStove()
    {
        Slots = new ProcedureStep();
        isSlotFilled = false;
        //progress bar
        ProgressbarHUD.SetActive(false);
        Progress.fillAmount = 0;
        SlotsIcon.sprite = itemHolder_grayScale;
        NormalVFX.SetActive(false);
        BurnedVFX.SetActive(false);
    }
    void stopFryer()
    {
        isCooking = false;
        StopCoroutine(CookingCoroutine);
    }
    void PickupDish()
    {

        isSlotFilled = false;
    }
    IEnumerator StartCooking(float maxCookingTime)
    {
        isInteractable = false;
        float timer = 0;
        float overcooktimer = maxCookingTime * .1f;
        float beforeOverCookTimer = maxCookingTime * .08f;
        float underCookTimer = maxCookingTime - (maxCookingTime * undercookPercentage);
        var maxTimer = maxCookingTime + overcooktimer + beforeOverCookTimer;
        isCooking = true;
        isPicked = false;
        //Set the timer visuals
        HUD.SetActive(true);
        SlotsHUD.SetActive(false);
        ProgressbarHUD.SetActive(true);
        Progress.fillAmount = 0;
        RedZone.fillAmount = overcooktimer / maxTimer;
        GreenZone.fillAmount = RedZone.fillAmount + (beforeOverCookTimer / maxTimer);
        BlueZone.fillAmount = GreenZone.fillAmount + (maxCookingTime * undercookPercentage) / maxTimer;
        Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"Report; " +
            $"[RedZone{RedZone.fillAmount},{overcooktimer}]" +
            $"[GreenZone{GreenZone.fillAmount},{beforeOverCookTimer}]" +
            $"[BlueZone{BlueZone.fillAmount},{(maxCookingTime * undercookPercentage)}]"));

        dishStatus = ProcessStatus.None;
        while (timer < underCookTimer)
        {
            yield return null;
            timer += Time.deltaTime;
            Progress.fillAmount = timer / maxTimer;
        }
        isInteractable = true;
        Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"underCooked{timer}"));
        //under cooked
        dishStatus = ProcessStatus.UnderCooked;
        //can interact here onwards
        //Start Cooking 
        while (timer < maxCookingTime)
        {
            yield return null;
            timer += Time.deltaTime;
            Progress.fillAmount = timer / maxTimer;
        }
        Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"cooked{timer}"));
        //Cooking Done
        dishStatus = ProcessStatus.Fryed;
        if (PerkSystemManager.Instance.CheckPerkUnlocked(perkSystem_Value.Auto, 0)) stopFryer();
        while (timer < maxCookingTime + beforeOverCookTimer)
        {
            yield return null;
            timer += Time.deltaTime;
            Progress.fillAmount = timer / maxTimer;
        }
        Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"overcooked{timer}"));
        dishStatus = ProcessStatus.OverCooked;
        while (timer < maxTimer)
        {
            yield return null;
            timer += Time.deltaTime;
            Progress.fillAmount = timer / maxTimer;
        }
        Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"burned{timer}"));
        dishStatus = ProcessStatus.Burned;
        //Burned food
        isCooking = false;
    }
    public override void LoadLevelUpgrades()
    {
        var temp = Data.upgradeData[savedata.level];
        lvlUpgradeDurationPercentage = temp.GetUpgradeValue(E_ValueName.Speed) / 100;
    }
}
