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
    public ASPFGrid Navmesh;
    public ASPFGrid AINavmesh;
    public RecipeBook Stovebook;
    public RecipeBook book;

    public List<GameObject> KitchenArea, RoomArea;
    public PlayableAreas CurrentArea;

    public GameObject SaveSpawnerParent;

    private Dictionary<string, ObjectLevelDetails> upgradableObjects;

    public static event Action<string, int> LevelupEvent;

    public GameState GetGameState { get { return m_GameState; } }

    public Bill currentDayBill;

    private int currentDaysIncome;

    //TODO: Alist of unlocked ingredients for proficiency system--Done
    public List<IngredientType> unlockedIngredient { get => m_UserDataLocal.ingredientsUnlocked; }

    public List<Dishes> CurrentDaysDishes;

    

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
        book.init();
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
        PerkSystemManager.Instance.init();
        ProficiencySystem.Instance.init();
        HUDManagerDNDL.Instance.AddCurrencyVisual(m_UserDataLocal.CurrencyAmount);
        HUDManagerDNDL.Instance.SetStarsVisual();
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
    }
    public void NewSave()
    {
        GameSaveDNDL.Instance.NewGame();
        NewSaveSetup();
        if (!DisableFTUT)
            StartCoroutine(WaitForState(isGamePlay, FTUT_Phase1));
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
        return m_UserDataLocal.Stars;
    }
    public int GetDifficultyBonus()
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
            case 6: return 5;
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
        var dishes = book.GetAllThePossibleDishes(procedureStep);
        return dishes;
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
        CurrentDaysDishes=CurrentSelected;
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
    public bool DisableFTUT;
    private GameObject FTUT_Refrigerator;
    private GameObject FTUT_ChoppingBoard;
    private GameObject FTUT_Stove;
    private GameObject FTUT_PlateTray;
    private NPC.NPC FTUT_Npc;
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
                                        //Move Cam to PC
                                        // Part 2 Ftut Starts
                                        ///Part 2
                                        //use the money in the room to purchase ingredient
                                        //player level msg
                                        //restarunt Rating msg
                                        //day end Bills
                                        ///------
                                    }, true);
                                });
                            });
                        });
                    }, false, true);
                });
            });
        });
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
}

