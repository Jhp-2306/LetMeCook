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
    //private void Start()//testing purpose
    //{
    //    Init();
    //}
    #region Save Mechanics
    public override void ReadFromSave(SaveDataTemplate _data)
    {
        base.ReadFromSave(_data);
        ///Any additional init/data distribution code here

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
        base.Init(EquipmentType.Frying_pan,item);
        ///Any additional init/data distribution code here
        Slots = new ProcedureStep();
        isSlotFilled = false;
        HUD.SetActive(false);
        isPicked = true;
        isCooking = false;
        isInteractable = true;
        ///in-between here
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
            SlotsUpdate();
            isSlotFilled=true;
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
        int idx = 0;
        if (isSlotFilled)
        {
            SlotsIcon.sprite = AssetLoader.Instance.GetIngredientIcon(Slots.Ingredient, Slots.processed);
            idx++;
        }
    }
    public override bool IsInteractionSatisfied()
    {
        return isInteractable;
    }
    public override void OnClick()
    {
        //Picking Mechanics
        if (isCooking && !isPicked)
        {
            //Stop the coroutine there and pick the dish
            isPicked = true;
            isCooking = false;
            StopCoroutine(CookingCoroutine);
            PickupDish();
            
            Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"Done Cooking Dish-- status{Slots.processed}"));
        }
        if (!isCooking && !isPicked)
        {
            isPicked = true;
            StopCoroutine(CookingCoroutine);
            PickupDish();
            //isSlotFilled = false;
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
            //Start Cooking
            //Check for the Output
            //var dish = GameDataDNDL.Instance.GetStoveDish(Slots);
            //var cooktimer = GameDataDNDL.Instance.GetCookingTime(dish);
            Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"Start Frying,{defaultCookingTime}"));
            CookingCoroutine = StartCoroutine(StartCooking(defaultCookingTime));
        }
        else
        {
            Debug.Log(CustomLogs.CC_TagLog("Frying Pan", "Not Enough Ingredient Added"));
        }
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
}
