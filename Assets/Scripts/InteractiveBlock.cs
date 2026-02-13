using Constants;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class InteractiveBlock : MonoBehaviour, IInteractable
{
    //public int TableID;
    //public string TableName;
    //
    // can remove this variable not in use

    public enum PlacementTags
    {
        Kitchen,
        Room
    }
    public EquipmentType equipmentType;
    [SerializeField] private Transform LookAtMe;
    public List<Vector3> ActiveGridPositions;
    public PlacementTags Tag;
    public SO_EquipmentData Data;
    protected bool m_isTableEmpty;
    protected SaveDataTemplate savedata;
    public int CellSize = 2;
    public bool isGizmos;
    public bool PreSpawn;
    public Transform GetLookPos()
    {
        return LookAtMe;
    }
   
    public virtual bool IsInteractionSatisfied()
    {
        return false;
    }
    public bool isTableEmpty() => Data == null;

    ///this need clean up
    public string GetInteractableName() => Data != null ?
        GameDataDNDL.Instance.GetPlayer().isHandsfull ?
        Data.HandFullDisplayFuntionName : Data.DisplayFuntionName : "none";
    public string GetHoldInteractableName() => Data != null ?
        !Data.HoldFuntionName.Equals("-")? Data.HoldFuntionName : "none": "none";
    public virtual void Init(EquipmentType _equip = EquipmentType.none, string _prefabID = "")
    {
        savedata = new SaveDataTemplate();
        if (_equip == EquipmentType.Furnitures)
            savedata.id = GameSaveDNDL.GenerateId(_prefabID);
        else
            savedata.id = GameSaveDNDL.GenerateId(_equip.ToString());
        savedata.Type = _equip;
        savedata.PrefabString = _prefabID;
        savedata.Position = transform.position;
        savedata.Rotation = transform.rotation;
    }
    public virtual void ReadFromSave(SaveDataTemplate _data)
    {
        savedata = new SaveDataTemplate();
        savedata = _data;
        transform.position = _data.Position;
        transform.rotation = _data.Rotation;
        //update the Grid
        InputManager.Instance.UpdateGridPositions(transform.position, LookAtMe.position);
    }
    public virtual void OnClick()
    {

    }
    public virtual void OnHold()
    {

    }

    public List<Vector3> GetBlockCells(Vector3 centerPos)
    {
        return null;
    }

    public bool IsInteractable()
    {
        throw new System.NotImplementedException();
    }

    public EquipmentType GetEquipmentType()
    {
        throw new System.NotImplementedException();
    }

    public SO_EquipmentData GetEquipmentData()
    {
        throw new System.NotImplementedException();
    }

    public void MoveTheObject(Vector3 pos)
    {
        transform.position = pos;
    }

    private void OnDrawGizmos()
    {
        if (!isGizmos) return;
        foreach (var pos in ActiveGridPositions)
        {
            Gizmos.DrawCube(transform.TransformPoint(pos), Vector3.one * 2);
        }
    }
}

