using Constants;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class InteractiveBlock : MonoBehaviour,IInteractable
{
    //public int TableID;
    //public string TableName;
    public EquipmentType equipmentType;
    [SerializeField] private Transform LookAtMe;
    //public Material myMaterial;

    public SO_EquipmentData Data;
    //[SerializeField] GameObject ItemParent;
    protected bool m_isTableEmpty;
    protected SaveDataTemplate savedata;
    public Transform GetLookPos()
    {
        return LookAtMe;
    }
    public void Awake()
    {
    }
    public void TableDataRefresh()
    {
       
    }
    public virtual bool IsInteractionSatisfied(){
            return false;
    }
    public bool isTableEmpty() => Data == null;
    public string GetInteractableName() => Data!=null?
        GameDataDNDL.Instance.GetPlayer().isHandsfull?
        Data.HandFullDisplayFuntionName : Data.DisplayFuntionName:"none";
    public virtual void Init()
    {
        savedata = new SaveDataTemplate();
        //savedata.id = GameSaveDNDL.GenerateId(EquipmentType.Refrigerator.ToString());
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
        //transform.position = new Vector3(SafeAreaMin.position.x <= Mathf.Abs(pos.x) && SafeAreaMax.position.x >= Mathf.Abs(pos.x) ? pos.x : transform.position.x, pos.y,
        //  SafeAreaMin.position.z <= Mathf.Abs(pos.z) && SafeAreaMax.position.z >= Mathf.Abs(pos.z) ? pos.z : transform.position.z);
        transform.position = pos; 
    }
}

