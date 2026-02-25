using Constants;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Stove : InteractiveBlock
{
    //4 Slots of ingredient
    //start to cook for timer
    public List<ProcedureStep> Slots;
    private const int MaxSlots = 4;
    //private float OverCookaddOnTimer = 20f;
    //private float OverCookBeforeAddOn = 30f;
    private float undercookPercentage = 0.08f;
    private float overcookPercentage = 0.1f;
    private float perfectcookingPercentage = 0.08f;
    //Hud
    public Image Progress;
    public Image RedZone;
    public Image GreenZone;
    public Image BlueZone;
    public Image TickMark;
    public List<Image> SlotsIcon;
    public GameObject HUD, ProgressbarHUD, SlotsHUD, DoneHUD;
    ProcessStatus dishStatus;
    bool isCooking;
    bool isPicked;
    private bool isInteractable;
    Coroutine CookingCoroutine;
    Dishes Currentcookingdish;
    float CurrentcookingdishPrice;
    float CurrentcookingdishPriceMulti;
    private void Start()//testing purpose
    {
        //Init();
    }
    public override void ReadFromSave(SaveDataTemplate _data)
    {
        base.ReadFromSave(_data);
        Slots = new List<ProcedureStep>();
        HUD.SetActive(false);
        isPicked = true;
        isCooking = false;
        isInteractable = true;
        //BeforeSaving();
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
    }

    public override void Init(EquipmentType _equip = EquipmentType.none, string item = "")
    {
        base.Init(EquipmentType.Stove, item);
        Slots = new List<ProcedureStep>();
        HUD.SetActive(false);
        isPicked = true;
        isCooking = false;
        isInteractable = true;
        BeforeSaving();
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
    }
    void BeforeSaving()
    {
        GameSaveDNDL.Instance.AddSaveData(savedata);
    }
    public void AddIngredient(Ingredient ingredient)
    {
        if (Slots.Count < MaxSlots)
        {
            Slots.Add(ingredient.ingrendient);
            //After Adding Ingredient inHandItem Removeds
            GameDataDNDL.Instance.GetPlayer().RemoveFromHand();
            SlotsUpdate();
        }
        else
        {
            Debug.Log(CustomLogs.CC_TagLog("Stove", "Failed To Add Ingredient-- Max Slots Filled"));
        }
    }
    public void SlotsUpdate()
    {
        HUD.SetActive(true);
        SlotsHUD.SetActive(true);
        ProgressbarHUD.SetActive(false);
        DoneHUD.SetActive(false);
        int idx = 0;
        foreach (var t in Slots)
        {
            SlotsIcon[idx].sprite = AssetLoader.Instance.GetIngredientIcon(t.Ingredient, t.processed);
            idx++;
        }
    }
    public override bool IsInteractionSatisfied()
    {
        return isInteractable;
    }
    public override void OnClick()
    {
        if (isCooking && !isPicked)
        {
            isCooking = false;
            StopCoroutine(CookingCoroutine);
            ProgressbarHUD.SetActive(false);
            DoneHUD.SetActive(true);
            switch (dishStatus)
            {
                case ProcessStatus.UnderCooked:
                    TickMark.color = BlueZone.color;
                    break;
                case ProcessStatus.OverCooked: TickMark.color = RedZone.color; break;
                case ProcessStatus.Cooked: TickMark.color = GreenZone.color; break;
            }
            isPicked = tryPickupDish();
            Debug.Log(CustomLogs.CC_TagLog("Stove", $"Done Cooking Dish-- status{dishStatus}"));
        }
        if (!isCooking && !isPicked)
        {
            StopCoroutine(CookingCoroutine);
            isPicked = tryPickupDish();
            Debug.Log(CustomLogs.CC_TagLog("Stove", $"Done Cooking Dish-- status{dishStatus}"));
        }
        var player = GameDataDNDL.Instance.GetPlayer();
        if (player.isHandsfull && player.InHand.IGetType() == typeofhandheld.ingredients && isPicked)
        {
            AddIngredient(player.InHand.GetGameObject().GetComponent<Ingredient>());

            if (isFTUT)
            {

                //We Call the Cooking FTUT
                FTUT_Cooking();

            }
        }
        else
        {
            Debug.Log(CustomLogs.CC_TagLog("Stove", "Failed To Add Ingredient-- Player Dont Have Any ingredient to add"));
        }

    }

    public override void OnHold()
    {
        //base.OnHold();
        if (isCooking || !isPicked) return;
        if (Slots.Count > 1)
        {
            //Start Cooking
            //Check for the Output
            Currentcookingdish = GameDataDNDL.Instance.GetStoveDish(Slots);
            CurrentcookingdishPrice = GameDataDNDL.Instance.GetStoveDishPrice(Currentcookingdish);
            CurrentcookingdishPriceMulti = GameDataDNDL.Instance.GetStoveDishPriceMulti(Currentcookingdish, Slots);
            var cooktimer = GameDataDNDL.Instance.GetCookingTime(Currentcookingdish);
            Debug.Log(CustomLogs.CC_TagLog("Stove", $"Start Cooking{Currentcookingdish},{cooktimer},{CurrentcookingdishPrice}{CurrentcookingdishPriceMulti}"));
            CookingCoroutine = StartCoroutine(StartCooking(cooktimer));
            DoneHUD.SetActive(false);
            if (isFTUT)
            {
                Debug.Log(CustomLogs.CC_TagLog("Stove", "init phase 3"));
                GameDataDNDL.Instance.FTUT_Phase3();
            }
        }
        else
        {
            Debug.Log(CustomLogs.CC_TagLog("Stove", "Not Enough Ingredient Added"));
        }
    }

    bool tryPickupDish()
    {
        var player = GameDataDNDL.Instance.GetPlayer();
        if (player.InHand != null && player.InHand.GetGameObject().GetComponent<Plate>() != null)
        {
            if (player.InHand.GetGameObject().GetComponent<Plate>().isPlateEmpty)
            {
                GetFoodcookMulti();
                player.InHand.GetGameObject().GetComponent<Plate>().pickupDish(Currentcookingdish, CurrentcookingdishPrice, CurrentcookingdishPriceMulti);
                Slots = new List<ProcedureStep>();
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
    IEnumerator StartCooking(float maxCookingTime)
    {
        isInteractable = false;
        float timer = 0;
        float underCookTimer = maxCookingTime - (maxCookingTime * undercookPercentage);//First Point from here on its undercooked
        float beforeOverCookTimer = maxCookingTime * perfectcookingPercentage;//third Point from here on its overcooked point
        float overcooktimer = maxCookingTime * overcookPercentage;
        var maxTimer = maxCookingTime + overcooktimer + beforeOverCookTimer;//fouth Point from here on its burnt point
        isCooking = true;
        isPicked = false;
        //Set the timer visuals
        HUD.SetActive(true);
        SlotsHUD.SetActive(false);
        DoneHUD.SetActive(false);
        ProgressbarHUD.SetActive(true);
        Progress.fillAmount = 0;
        RedZone.fillAmount = overcooktimer / maxTimer;
        GreenZone.fillAmount = RedZone.fillAmount + (beforeOverCookTimer / maxTimer);
        BlueZone.fillAmount = GreenZone.fillAmount + (maxCookingTime * undercookPercentage) / maxTimer;
        Debug.Log(CustomLogs.CC_TagLog("Stove", $"Report; " +
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
        Debug.Log(CustomLogs.CC_TagLog("Stove", $"underCooked{timer}"));
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
        Debug.Log(CustomLogs.CC_TagLog("Stove", $"cooked{timer}"));
        //Cooking Done

        dishStatus = ProcessStatus.Cooked;
        if (isFTUT)
        {
            if (GameDataDNDL.Instance.isPhase3done)
            {
                GameDataDNDL.Instance.FTUT_Phase4();
            }
            GameDataDNDL.Instance.isPhase3done = true;
            isFTUT = false;
            while (!isFTUT)
            {
                yield return null;
            }
        }
        while (timer < maxCookingTime + beforeOverCookTimer)
        {
            yield return null;
            timer += Time.deltaTime;
            Progress.fillAmount = timer / maxTimer;
        }
        Debug.Log(CustomLogs.CC_TagLog("Stove", $"overcooked{timer}"));
        dishStatus = ProcessStatus.OverCooked;
        while (timer < maxTimer)
        {
            yield return null;
            timer += Time.deltaTime;
            Progress.fillAmount = timer / maxTimer;
        }
        Debug.Log(CustomLogs.CC_TagLog("Stove", $"burned{timer}"));
        dishStatus = ProcessStatus.Burned;
        //Burned food
        isCooking = false;
    }

    bool isFTUT;
    public void init_FTUT()
    {
        isFTUT = true;
    }

    public void FTUT_Cooking()
    {
        HUDManagerDNDL.Instance.SetTutorialHUD("Explain Stove Mechanics", () =>
        {
            HUDManagerDNDL.Instance.SetTutorialHUD("Now Hold to Start Cooking", InputManager.Instance.Interactionbtn.gameObject, () =>
                {
                    InputManager.Instance.Interactionbtn.AddEventsspc(null,() =>
                    {
                        HUDManagerDNDL.Instance.TutorialHUDDisableFocus();

                    });
                }
                );
        });
    }
}
