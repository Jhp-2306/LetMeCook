using ASPathFinding;
using Constants;
using NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;


public class GameDataDNDL : Singletonref<GameDataDNDL>
{
    public Material Selected;
    public Material normal;
    public DateTime StartTime;
    public DateTime PreviousGameCloseTime;


    private KitchenState m_KitchenState;
    private GameState m_GameState;

    [SerializeField]
    private Player m_player;
    [SerializeField]
    private GameObject m_FreeCamRig;
    [SerializeField]
    private GridMaker grid;
    private UserDataLocal m_UserDataLocal;

    public List<InteractiveBlock> TableObjects;
    public SO_EquipmentDataList ItemList;
    public SO_EquipmentDataList FurnitureItemList;

    public ASPFGrid Navmesh;
    public ASPFGrid AINavmesh;
    public RecipeBook Stovebook;
    public RecipeBook plateBook;
    public RecipeBook FryerBook;

    public List<GameObject> KitchenArea, RoomArea;
    public PlayableAreas CurrentArea;

    public GameObject SaveSpawnerParent;

    private Dictionary<string, ObjectLevelDetails> upgradableObjects;

    public static event Action<string, int> LevelupEvent;

    public GameState GetGameState { get { return m_GameState; } }

    public Bill currentDayBill;

    private int currentDaysIncome;

    //TODO: A list of unlocked ingredients for proficiency system--Done
    public List<IngredientType> unlockedIngredient { get => m_UserDataLocal.ingredientsUnlocked; }

    public List<Dishes> CurrentDaysDishes;

    public List<Dishes> GetUnlockedDishes
    {
        get =>
            ProficiencySystem.Instance.getUnlockDish();
    }


    #region Init
    private void Awake()
    {
        base.Awake();
        //Variable init
        m_KitchenState = 0;
        m_GameState = GameState.PauseGame;
        m_UserDataLocal = new UserDataLocal();
        CurrentArea = PlayableAreas.Kitchen;
        //script inits

        grid.Init();
        Navmesh.Init();
        AINavmesh.Init();
        Stovebook.init();
        plateBook.init();
        FryerBook.init();
    }
    private void OnApplicationQuit()
    {
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
        TimeManagementDNDL.OnKitchenOpen -= SelectDish;
        TimeManagementDNDL.EndOfTheDay -= OnDayEndBilling;
    }

    private void OnDestroy()
    {
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
        TimeManagementDNDL.OnKitchenOpen -= SelectDish;
        TimeManagementDNDL.EndOfTheDay -= OnDayEndBilling;

    }
    private void Start()
    {
        //for Testing Purpose only
        if (!SaveData.Instance.LoadInstance())
        {
            //Failed to get the local data
            SaveData.Instance.NewGameSaveData();
        }
        StartCoroutine(WaitForAddressableToLoad());
        m_UserDataLocal = SaveData.Instance.LocalData;
        if (m_UserDataLocal.ingredientsUnlocked == null)
        {
            m_UserDataLocal.ingredientsUnlocked = new List<IngredientType>();
        }
        m_UserDataLocal.ingredientsUnlocked.Add(IngredientType.Tomato);
        upgradableObjects = new Dictionary<string, ObjectLevelDetails>();

        //All the save Data and pre init-here-
        PerkSystemManager.Instance.init();
        ProficiencySystem.Instance.init();

        HUDManagerDNDL.Instance.AddCurrencyVisual(m_UserDataLocal.CurrencyAmount);
        HUDManagerDNDL.Instance.SetStarsVisual();
        HUDManagerDNDL.Instance.DisableHUD();
        TimeManagementDNDL.OnKitchenOpen -= SelectDish;
        TimeManagementDNDL.OnKitchenOpen += SelectDish;
        TimeManagementDNDL.EndOfTheDay -= OnDayEndBilling;
        TimeManagementDNDL.EndOfTheDay += OnDayEndBilling;
        BeforeSaving();
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
        //OnDayEndBilling ();
    }
    IEnumerator WaitForAddressableToLoad()
    {
        AssetLoader.Instance.Addressableinit();
        yield return new WaitUntil(AssetLoader.Instance.GetisAddressableLoaded);
        //New Save Setting
        if (m_UserDataLocal != null)
        {
            if (!m_UserDataLocal.isNGDataSet)
            {
                NewSave();
                yield return null;
            }
            //Debug.Log($"check {SaveData.Instance.LocalData == null}");
            else
                foreach (var t in SaveData.Instance.saveDataType.GetAllTheData())
                {
                    Debug.Log($"Save Template {t.id}{t.Type},{t.PrefabString}");
                    //here spawn the Tools
                    var go = Instantiate(AssetLoader.Instance.GetEquipmetPrefab(t.PrefabString));
                    if (go.GetComponent<InteractiveBlock>() != null)
                    {
                        go.GetComponent<InteractiveBlock>().ReadFromSave(t);
                        go.transform.SetParent(SaveSpawnerParent.transform);
                    }
                }
        }
    }

    #endregion

    #region Save
    public void ContinueSave()
    {
        m_GameState = GameState.PlayGame;
        HUDManagerDNDL.Instance.Init();
    }
    public void NewSave()
    {
        GameSaveDNDL.Instance.NewGame();
        NewSaveSetup();
        if (!DisableFTUT)
        {
            StartCoroutine(WaitForState(isGamePlay, FTUT_Phase1)); //Main Line Of code

            //Testing Line
            //StartCoroutine(WaitForState(isGamePlay,()=> { 
            //    isFTUT=true;
            //    FTUT_Phase5(); }));
        }
        //FTUT_Phase1();
    }
    void BeforeSaving()
    {
        SaveData.Instance.LocalData = m_UserDataLocal;
        //GameSaveDNDL.Instance.AddSaveData(savedata);
    }
    bool isGamePlay() => m_GameState == GameState.PlayGame;
    #endregion

    #region Currency,Star,Unlock_Conditions
    public void AddCurrency(int amount)
    {
        m_UserDataLocal.CurrencyAmount += amount;
        currentDaysIncome += amount;
        HUDManagerDNDL.Instance.AddCurrencyVisual(m_UserDataLocal.CurrencyAmount);
    }
    public bool DoIHaveEnoughCurrency(int amount)
    {
        var temp = m_UserDataLocal.CurrencyAmount;
        if (temp - amount > 0)
        {
            return true;

        }
        else return false;
    }
    public bool tryDeductingCurrency(int amount)
    {
        var temp = m_UserDataLocal.CurrencyAmount;
        if (temp - amount >= 0)
        {
            m_UserDataLocal.CurrencyAmount -= amount;
            HUDManagerDNDL.Instance.AddCurrencyVisual(m_UserDataLocal.CurrencyAmount);
            return true;
        }
        else return false;
    }
    public int GetStars()
    {
        var xp = m_UserDataLocal.StarsXP;
        if (xp < 100)
        {
            return 1;
        }
        if (xp < 200)
        {
            return 2;
        }
        if (xp < 800)
        {
            return 3;
        }
        if (xp < 1600)
        {
            return 4;
        }
        if (xp >= 1600)
        {
            return 5;
        }
        return 0;
    }
    public void AddStarXP(float Rating)
    {
        var stars = GetStars();
        switch (Rating)
        {
            case 3:
                m_UserDataLocal.StarsXP += 25;
                break;
            case 2.5f:
                m_UserDataLocal.StarsXP += stars >= 5 ? 22 : 17;
                break;
            case 2:
                m_UserDataLocal.StarsXP += stars >= 5 ? 20 : 15;
                break;
            case 1.5f:
                m_UserDataLocal.StarsXP += stars >= 5 ? 15 : 12;
                break;
            case 1:
                m_UserDataLocal.StarsXP += stars >= 5 ? 12 : 10;
                break;
            case 0.5f:
                m_UserDataLocal.StarsXP += stars >= 5 ? 5 : 2;
                break;
            case 0:
                switch (stars)
                {
                    case 0:
                        
                        break;
                    case 1:
                        m_UserDataLocal.StarsXP -= 1;
                        break;
                    case 2:
                        m_UserDataLocal.StarsXP -= 10;
                        break;
                    case 3:
                        m_UserDataLocal.StarsXP -= 12;
                        break;
                    case 4:
                        m_UserDataLocal.StarsXP -= 15;
                        break;
                    case 5:
                        m_UserDataLocal.StarsXP -= 17;
                        break;
                    default: break;
                }
                break;
        }
        
    }
    public float fillRate()
    {
        var star = GetStars();
        //Finish here
        return 0;
    }
    private int GetDifficultyBonus()
    {
        var stars = GetStars();
        switch (stars)
        {
            case 0: return 1;
            case 1: return 1;
            case 2: return 2;
            case 3: return 2;
            case 4: return 3;
            case 5: return 4;
            default: return 1;
        }
    }

    public void UnlockedIngredients(IngredientType type)
    {
        if (m_UserDataLocal.ingredientsUnlocked.Count == (int)IngredientType.count) return;//All ingredient unlocked
        if (m_UserDataLocal.ingredientsUnlocked == null)// init for value
        {
            m_UserDataLocal.ingredientsUnlocked = new List<IngredientType>();
            m_UserDataLocal.ingredientsUnlocked.Add(type);
        }
        else// confirm and add value to the list
        {
            if (!m_UserDataLocal.ingredientsUnlocked.Contains(type))
            {
                Debug.Log($"Adding,,,,{type} ");
                m_UserDataLocal.ingredientsUnlocked.Add(type);
            }
        }
    }
    #endregion

    #region Player and Cam
    public void SetPlayer(Player player)
    {
        m_player = player;
    }
    public Player GetPlayer()
    {
        return m_player;
    }
    public void SetFreeCamRig(GameObject player)
    {
        m_FreeCamRig = player;
    }
    public GameObject GetFreeCamRig()
    {
        return m_FreeCamRig;
    }
    #endregion

    #region Grid and NavMesh
    public GridMaker GetGrid() => grid;
    public void UpdateNavMesh(int x, int y, bool value)
    {
        Navmesh.UpdateIsWalkableSpecificCell(x, y, value);
    }

    public void UpdateNavMesh(Vector3 worldpos, bool value)
    {
        Navmesh.UpdateIsWalkableSpecificCell(worldpos, value);
    }
    public void UpdateAINavMesh(Vector3 worldpos, bool value)
    {
        AINavmesh.UpdateIsWalkableSpecificCell(worldpos, value);
    }
    public void SwitchArea(PlayableAreas area)
    {
        CurrentArea = area;
    }
    #endregion

    #region Dishes Data
    public Dishes GetStoveDish(List<ProcedureStep> procedureSteps)
    {
        return Stovebook.GetDishes(procedureSteps);
    }
    public float GetStoveDishPrice(Dishes dish)
    {
        return Stovebook.GetPrice(dish);
    }
    public float GetStoveDishPriceMulti(Dishes dish, List<ProcedureStep> procedureSteps)
    {
        return Stovebook.GetPriceMultipler(dish, procedureSteps);
    }
    public int GetCookingTime(Dishes dish)
    {
        return Stovebook.GetDishCookTime(dish);
    }

    public List<Recipes> GetAllThePossibleDishes(List<ProcedureStep> procedureStep)
    {
        var dishes = plateBook.GetAllThePossibleDishes(procedureStep);
        return dishes;
    }
    public Dishes GetFryerDish(ProcedureStep procedureSteps)
    {
        return FryerBook.GetDishes(new List<ProcedureStep> { procedureSteps, });
    }
    public float GetFryerDishPrice(Dishes dish)
    {
        return FryerBook.GetPrice(dish);
    }
    //Open Selection panel
    public void SelectDish()
    {
        //Pause Timer
        m_GameState = GameState.PauseGame;
        HUDManagerDNDL.Instance.ShowDishSelector();
    }
    public void StartTheDay(List<Dishes> CurrentSelected)
    {
        CurrentDaysDishes = CurrentSelected;
        m_GameState = GameState.PlayGame;
    }



    #endregion

    #region Equipments
    public Dictionary<string, ObjectLevelDetails> GetalltheLevels() => upgradableObjects;
    public void AddUpgradableObject(string id, ObjectLevelDetails lvl)
    {
        if (!upgradableObjects.TryAdd(id, lvl))
        {
            upgradableObjects[id] = lvl;
        }
    }
    public void LevelUpCallback(string id, int lvl)
    {
        LevelupEvent(id, lvl);
    }
    #endregion

    #region FTUT
    [Space(10)]
    [Header("FTUT")]
    public bool isFTUT;
    public bool DisableFTUT { get => SettingDNDL.Instance.FTUT; }
    private GameObject FTUT_Refrigerator;
    private GameObject FTUT_ChoppingBoard;
    private GameObject FTUT_Stove;
    private GameObject FTUT_PlateTray;
    [SerializeField] private GameObject FTUT_Pc;
    [SerializeField] private GameObject FTUT_MarketAppIcon;

    private NPC.NPC FTUT_Npc;
    [SerializeField] private GameObject RoomDoor;
    [SerializeField] private GameObject RoomDoorEntryPoint;
    public bool isDoorCallbackFlag;
    public void NewSaveSetup()
    {
        //Set Refrigerator
        FTUT_Refrigerator = ShopManager.Instance.PlaceEquipmentAtaPoint("Refrigerator", new Vector3(17, 0, 11), new Vector3());
        FTUT_Refrigerator.GetComponent<Refrigerator>().AddFTUTIngredient();
        //UnlockedIngredients(IngredientType.Tomato);
        //Set ChoppingBoard
        FTUT_ChoppingBoard = ShopManager.Instance.PlaceEquipmentAtaPoint("Chopping_Board", new Vector3(15, 0, 11), new Vector3());
        //Set Stove
        FTUT_Stove = ShopManager.Instance.PlaceEquipmentAtaPoint("Stove", new Vector3(13, 0, 11), new Vector3());
        //Set PlateTray
        FTUT_PlateTray = ShopManager.Instance.PlaceEquipmentAtaPoint("Platetray", new Vector3(11, 0, 11), new Vector3());
        //Set Bin
        ShopManager.Instance.PlaceEquipmentAtaPoint("Bin", new Vector3(9, 0, 11), new Vector3());
        m_UserDataLocal.isNGDataSet = true;
    }

    //First Time User Tutorial
    void FTUT_Phase1()
    {
        if (DisableFTUT) return;
        isFTUT = true;
        ///Part 1
        //Start with intro msg
        HUDManagerDNDL.Instance.SetTutorialHUD("hello Welcome to LMC", () =>
        {
            HUDManagerDNDL.Instance.SetTutorialHUD("Tap on the Floor to Move", () =>
            {
                //Player Movement
                HUDManagerDNDL.Instance.SetTutorialHUD(FTUT_Refrigerator.transform, () =>
                {
                    InputManager.Instance.FTUT_MovePlayer(FTUT_Refrigerator.GetComponent<InteractiveBlock>(), () =>
                    {
                        HUDManagerDNDL.Instance.SetTutorialHUD(InputManager.Instance.Interactionbtn.transform, () =>
                        {
                            InputManager.Instance.Interactionbtn.InvokeClick();
                            Debug.Log("Open from here");
                            //pick ingredient from fridge//pick ingredient from fridge
                            //HUDManagerDNDL.Instance.SetTutorialHUD(InputManager.Instance.Interactionbtn.transform, () =>
                            {
                                HUDManagerDNDL.Instance.SetTutorialHUD(/*InputManager.Instance.Interactionbtn.GetItemSlots()*/"Pick a Tomato", () =>
                                {
                                    HUDManagerDNDL.Instance.SetTutorialHUD(InputManager.Instance.Interactionbtn.GetItemSlots(), () =>
                                    {
                                        //InputManager.Instance.Interactionbtn.OnPressUp();
                                        InputManager.Instance.Interactionbtn.GetItemSlots().GetComponent<InventoryItemHolder>().onclick();
                                        HUDManagerDNDL.Instance.SetTutorialHUD("Now Go to Chopping Board", () =>
                                        {
                                            HUDManagerDNDL.Instance.SetTutorialHUD(FTUT_ChoppingBoard.transform, () =>
                                            {
                                                InputManager.Instance.FTUT_MovePlayer(FTUT_ChoppingBoard.GetComponent<InteractiveBlock>(), () =>
                                                {
                                                    HUDManagerDNDL.Instance.SetTutorialHUD(InputManager.Instance.Interactionbtn.transform, () =>
                                                    {
                                                        //chop the ingredient
                                                        Debug.Log("Open from here");
                                                        //InputManager.Instance.Interactionbtn.InvokeClick();
                                                        FTUT_ChoppingBoard.GetComponent<ChoppingBoard>().init_FTUT();
                                                    }, false, true);
                                                });
                                            });
                                        });
                                    }, false, true);
                                });
                            }/*, true, true);*/
                        }, false, true);
                    });
                });
            });
        });




        //wait for the player to add on more ingredient
        //stove FTUT
        //grab a plate
        //get the food out and give it to the customer
        ///-----

        ///Part 2
        //use the money in the room to purchase ingredient
        //player level msg
        //restarunt Rating msg
        //day end Bills
        ///------
    }
    public void FTUT_Phase2()
    {
        //throe it into stove
        //show the recipe for the tomato soup
        HUDManagerDNDL.Instance.SetTutorialHUD("Now Add it in the Stove", () =>
        {
            HUDManagerDNDL.Instance.SetTutorialHUD(FTUT_Stove.transform, () =>
            {
                InputManager.Instance.FTUT_MovePlayer(FTUT_Stove.GetComponent<InteractiveBlock>(), () =>
                {
                    HUDManagerDNDL.Instance.SetTutorialHUD(InputManager.Instance.Interactionbtn.transform, () =>
                     {
                         InputManager.Instance.Interactionbtn.InvokeClick();
                         HUDManagerDNDL.Instance.SetTutorialHUD("Add another Sliced Tomato now", () =>
                         {
                             FTUT_Stove.GetComponent<Stove>().init_FTUT();
                             FTUT_Npc = NPCManager.Instance.RequestFTUTNPC();
                         }, true);
                     }, false, true);
                });
            });
        });
    }
    public bool isPhase3done;
    public void FTUT_Phase3()
    {
        //throe it into stove
        //show the recipe for the tomato soup
        HUDManagerDNDL.Instance.SetTutorialHUD("Wait for the Perfect indicator, meanwhile Get a Plate from the Tray", () =>
        {
            HUDManagerDNDL.Instance.SetTutorialHUD(FTUT_PlateTray.transform, () =>
            {
                InputManager.Instance.FTUT_MovePlayer(FTUT_PlateTray.GetComponent<InteractiveBlock>(), () =>
                {
                    HUDManagerDNDL.Instance.SetTutorialHUD(InputManager.Instance.Interactionbtn.transform, () =>
                    {
                        InputManager.Instance.Interactionbtn.InvokeClick();
                        if (isPhase3done)
                        { FTUT_Phase4(); }
                        isPhase3done = true;
                    }, true, true);
                });
            });
        });
    }
    public void FTUT_Phase4()
    {
        HUDManagerDNDL.Instance.SetTutorialHUD("Pick the Dish", () =>
        {
            HUDManagerDNDL.Instance.SetTutorialHUD(FTUT_Stove.transform, () =>
            {
                InputManager.Instance.FTUT_MovePlayer(FTUT_Stove.GetComponent<InteractiveBlock>(), () =>
                {
                    HUDManagerDNDL.Instance.SetTutorialHUD(InputManager.Instance.Interactionbtn.transform, () =>
                    {
                        InputManager.Instance.Interactionbtn.InvokeClick();
                        //Giving the Dish to the npc
                        HUDManagerDNDL.Instance.SetTutorialHUD("Give the Dish to the  NPC", () =>
                        {
                            HUDManagerDNDL.Instance.SetTutorialHUD(FTUT_Npc.CurrentPlatform.transform, () =>
                            {
                                InputManager.Instance.FTUT_MovePlayer(FTUT_Npc.CurrentPlatform.GetComponent<InteractiveBlock>(), () =>
                                {
                                    HUDManagerDNDL.Instance.SetTutorialHUD("wow you earned some currency", () =>
                                    {

                                        FTUT_Phase5();

                                    });
                                });
                            });
                        });
                    }, false, true);
                });
            });
        });
    }
    void FTUT_Phase5()
    {
        //Move the Player To the Room
        HUDManagerDNDL.Instance.SetTutorialHUD("now Move to Your Room", () =>
        {
            InputManager.Instance.DisablePlayerMovement(true);
            //Move Cam to Room Enteranc
            InputManager.Instance._cameraControle.SetTarget(RoomDoor, () =>
            {
                InputManager.Instance.DisablePlayerMovement(false);
                HUDManagerDNDL.Instance.SetTutorialHUD(RoomDoor.transform, () =>
                {
                    InputManager.Instance._cameraControle.SetTarget(GetPlayer().gameObject);
                    var pos = GetGrid().GetRoundAnchorPositionFromWorldPosition(RoomDoor.transform.position);
                    isDoorCallbackFlag = true;
                    InputManager.Instance.FTUT_MovePlayer(pos, () =>
                    {


                    });
                    InputManager.Instance.DisablePlayerMovement(true);
                }, true);
            });
        }, true);
    }
    public void FTUT_Phase6()
    {
        HUDManagerDNDL.Instance.SetTutorialHUD(FTUT_Pc.transform, () =>
        {
            InputManager.Instance.FTUT_MovePlayer(FTUT_Pc.GetComponent<InteractiveBlock>(), () =>
            {
                //StartCoroutine(DelayInvoke(0.75f, () => { 
                //}));
                HUDManagerDNDL.Instance.SetTutorialHUD(InputManager.Instance.Interactionbtn.transform, () =>
                {
                    InputManager.Instance.Interactionbtn.InvokeClick();
                    HUDManagerDNDL.Instance.SetTutorialHUD(new List<string>() { "This Your Computer Where you have different apps which will help you grow",
                        "Now Let Buy Some Ingredients" }, () =>
                    {
                        HUDManagerDNDL.Instance.SetTutorialHUD("Open Market App", FTUT_MarketAppIcon, () =>
                        {
                            //HUDManagerDNDL.Instance.TutorialHUDDisableFocus();
                            //HUDManagerDNDL.Instance.SetTutorialHUD("Add 1xCarrot, 1xTomato, 1xOnion", () =>
                            //{

                            //});
                        });

                    });
                }, false, true);
            });
        });
    }
    public void FTUT_Phase7()
    {

    }
    #endregion


    IEnumerator WaitForState(Func<bool> condition, Action callback)
    {
        var trigger = false;
        //while (!trigger) { 
        yield return new WaitUntil(condition);
        callback();
        //}
    }
    public IEnumerator DelayInvoke(float delaysec, Action callback)
    {
        //var trigger = false;
        //while (!trigger) { 
        yield return new WaitForSeconds(delaysec);
        callback();
        //}
    }
    void OnDayEndBilling()
    {
        //Save Data
        //pay the Bill
        //Bill
        m_GameState = GameState.PauseGame;
        currentDayBill = new Bill(equipmentOwne: upgradableObjects.Count,
                                  costPerEquipment: PerkSystemManager.Instance.GetPerkValue(perkSystem_Value.EquipmentOwnePrice),
                                  totalCustomerAttended: NPCManager.Instance.CurrentAICount,
                                  totalIncome: currentDaysIncome,
                                  incomeTax: PerkSystemManager.Instance.GetPerkValue(perkSystem_Value.IncomeTaxRate),
                                  difficulty: GetStars(),
                                  difficultyBonus: GetDifficultyBonus(),
                                  randomGovtTax: 7);
        //Failed to pay result in Save Faileds
        HUDManagerDNDL.Instance.BillUi.SetBill(currentDayBill);
    }
    public void OnbillPayment()
    {
        if (tryDeductingCurrency(currentDayBill.GetTotal()))
        {
            //Start A new day 
            //save data
        }
        else
        {
            // issue Bankruptcy 
            // check for prestige
            // if Presitige then open popup for it 
            // else move to a new save popup
            // and Save Data
        }
    }

    public void TesterToolCommandGiveIngredientToPlayer(IngredientType type)
    {
        var go = Instantiate(AssetLoader.Instance.itemPrefab);
        go.GetComponent<Ingredient>().Setup(AssetLoader.Instance.GetIngredientSO(type));
        GetPlayer().PickSomeThing(go.GetComponent<IHandHeld>(), go);
    }
}

