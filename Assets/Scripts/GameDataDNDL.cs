using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
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

    private UserDataLocal m_UserDataLocal;

    public List<Table> TableObjects;
    public SO_EquipmentDataList ItemList;

    private void Awake()
    {
        base.Awake();
        GameSaveDNDL.DataUpdateBeforeSave -= SaveTableData;
        GameSaveDNDL.DataUpdateBeforeSave += SaveTableData;
    }
    private void OnApplicationQuit()
    {
        GameSaveDNDL.DataUpdateBeforeSave -= SaveTableData;
    }

    private void OnDestroy()
    {
        GameSaveDNDL.DataUpdateBeforeSave -= SaveTableData;
    }
    private void Start()
    {
        m_KitchenState = 0;
        m_GameState = GameState.PlayGame;
        SaveData.Instance.LoadInstance();
        m_UserDataLocal = new UserDataLocal();
        m_UserDataLocal = SaveData.Instance.LocalData;
        Debug.Log($"check {SaveData.Instance.LocalData == null}");
        UpdateTableData(); 
    }

    void UpdateTableData()
    {
        List<TableData> temp = new List<TableData>();
        temp = m_UserDataLocal.tableData;
        if (temp != null)
            for (int i = 0; i < temp.Count; i++)
            {
                //TableData[i].TableName = temp[i].;
                var t = GetShopItemDetailsFromString(temp[i].TableTop);
                if(t != null)
                {
                    TableObjects[i].SetOnTable(t.Prefab,t.name,t.Type);
                }
            }
    }

    void SaveTableData()
    {
        int idx = 0;
        List<TableData> datas = new List<TableData>();
        foreach( var t in TableObjects)
        {
            var table=new TableData(t.TableID,t.equipmentType.ToString(),t.isTableEmpty(),null,null);
            SaveData.Instance.LocalData.tableData.Add(table);
        }
        Debug.Log($"check {SaveData.Instance.LocalData.tableData.Count}");
        //SaveData.Instance.LocalData.SetTableData(datas);
    }

    equipmentData GetShopItemDetailsFromString(string equipmentType)
    {
        foreach (var t in ItemList.equipmentDataList)
        {
            if (t.Type.ToString() == equipmentType)
            {
                return t;
            }
        }
        return null;
    }
    //ShopManager.ShopItems GetShopItemFromEquipmenttype
    private void OnDrawGizmos()
    {
        //for (int i = 0; i < TableData.Count; i++)
        //{
        //    TableData[i].TableID = i;
        //    TableData[i].gameObject.name = $"Table {i}";
        //}
    }

    public void SetPlayer(Player player)
    {
        m_player=player;
    }
    public Player GetPlayer()
    {
        return m_player;
    }
}
