using ASPathFinding;
using Constants;
using System;
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
    public RecipeBook book;

    public List< GameObject > KitchenArea,RoomArea;
    public PlayableAreas CurrentArea;
    private void Awake()
    {
        base.Awake();
        //Variable init
        m_KitchenState = 0;
        m_GameState = GameState.PlayGame;
        m_UserDataLocal = new UserDataLocal();
        m_UserDataLocal = SaveData.Instance.LocalData;
        CurrentArea = PlayableAreas.Kitchen;
        //script inits
        Navmesh.Init();
        AINavmesh.Init();
        book.init();
    }
    private void OnApplicationQuit()
    {
        
    }

    private void OnDestroy()
    {
       
    }
    private void Start()
    {
       
        SaveData.Instance.LoadInstance();       
        //Debug.Log($"check {SaveData.Instance.LocalData == null}");
        foreach (var t in SaveData.Instance.saveDataType.GetAllTheData())
        {
            Debug.Log($"Save Template {t.id}{t.Type}");
            //here spawn the Tools
            foreach(var temp in ItemList.equipmentDataList)
            {
                if(t.Type == temp.Type)
                {
                    var go = Instantiate(temp.Prefab);
                    if (go.GetComponent<InteractiveBlock>() != null)
                    { go.GetComponent<InteractiveBlock>().ReadFromSave(t); }
                }
            }
        }
       
       
    }

  

    public void SetPlayer(Player player)
    {
        m_player=player;
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
    public void UpdateNavMesh(int x,int y,bool value)
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
    public Dishes GetDish(List<ProcedureStep> procedureSteps)
    {
        return book.GetDishes(procedureSteps);
    }
    public int GetCookingTime(Dishes dish)
    {
        return book.GetDishCookTime(dish);
    }

    public void SwitchArea(PlayableAreas area)
    {
        CurrentArea= area;
    }
}
