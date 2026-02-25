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


    private void Awake()
    {
        base.Awake();
        //Variable init
        m_KitchenState = 0;
        m_GameState = GameState.PlayGame;
        m_UserDataLocal = new UserDataLocal();
        CurrentArea = PlayableAreas.Kitchen;
        //script inits
        Navmesh.Init();
        AINavmesh.Init();
        Stovebook.init();
        book.init();
    }
    private void OnApplicationQuit()
    {
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
    }

    private void OnDestroy()
    {
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;

    }
    private void Start()
    {
        SaveData.Instance.LoadInstance();
        StartCoroutine(WaitForAddressableToLoad());
        m_UserDataLocal = SaveData.Instance.LocalData;
        HUDManagerDNDL.Instance.AddCurrencyVisual(m_UserDataLocal.CurrencyAmount);
        BeforeSaving();
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
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
                NewSaveSetup();
                FTUT_Phase1();
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
                    { go.GetComponent<InteractiveBlock>().ReadFromSave(t); }
                }
        }
    }

    void BeforeSaving()
    {
        SaveData.Instance.LocalData = m_UserDataLocal;
        //GameSaveDNDL.Instance.AddSaveData(savedata);
    }
    public void AddCurrency(int amount)
    {
        m_UserDataLocal.CurrencyAmount += amount;
        HUDManagerDNDL.Instance.AddCurrencyVisual(m_UserDataLocal.CurrencyAmount);
    }


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
    public void SwitchArea(PlayableAreas area)
    {
        CurrentArea = area;
    }

    #region FTUT
    public bool isFTUT;
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
        //Set ChoppingBoard
        FTUT_ChoppingBoard = ShopManager.Instance.PlaceEquipmentAtaPoint("Chopping_Board", new Vector3(15, 0, 11), new Vector3());
        //Set Stove
        FTUT_Stove = ShopManager.Instance.PlaceEquipmentAtaPoint("Stove", new Vector3(13, 0, 11), new Vector3());
        //Set PlateTray
        FTUT_PlateTray = ShopManager.Instance.PlaceEquipmentAtaPoint("Platetray", new Vector3(11, 0, 11), new Vector3());
        //Set Bin
        m_UserDataLocal.isNGDataSet = true;
    }

    //public GameObject GetFTUTTarget(int _case)
    //{
    //    switch (_case)
    //    {
    //        case 0: return FTUT_Refrigerator;
    //        default: return null;
    //    }
    //}
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
                    HUDManagerDNDL.Instance.SetTutorialHUD("Add another Sliced Tomato now", () => {
                    FTUT_Stove.GetComponent<Stove>().init_FTUT();
                      FTUT_Npc=  NPCManager.Instance.RequestFTUTNPC();
                    },true);
                },false,true);
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
                        if(isPhase3done)
                        { FTUT_Phase4(); }
                        isPhase3done= true;
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
                                InputManager.Instance.FTUT_MovePlayer(FTUT_Npc.CurrentPlatform.GetComponent<InteractiveBlock>(), () => { 
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
                                },true);
                                });
                            });
                        });
                    }, false, true);
                });
            });
        });
    }
    #endregion
    void OnDayEndBilling()
    {
        //pay the Bill
        //Bill
        //Calculate the equipment usage and electric price
        //Gst
        //Tax For no reason
        //Failed to pay result in Save Faileds
    }
}
