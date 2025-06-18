using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public int TableID;
    public string TableName;
    public EquipmentType equipmentType;
    public Transform LookAtMe;
    public Material myMaterial;

    public SO_EquipmentData Data;
    [SerializeField] GameObject ItemParent;
    bool m_isTableEmpty;
    public Transform GetLookPos()
    {
        return LookAtMe;
    }
    public void Awake()
    {
        myMaterial = GetComponent<MeshRenderer>().sharedMaterials[0];
        TableDataRefresh();
    }
    public void TableDataRefresh()
    {
        if (GetComponentInChildren<IInteractable>() != null)
            Data = GetComponentInChildren<IInteractable>().GetEquipmentData();
    }
    public bool IsInteractionSatisfied(){
        if (this.GetComponentInChildren<IInteractable>() != null)
        {
            return this.GetComponentInChildren<IInteractable>().IsInteractable();
        }else if (isTableEmpty())
        {
            return true;
        } else
            return false;
    }
    public bool isTableEmpty() => Data == null;
    public string GetInteractableName() => Data!=null?
        GameDataDNDL.Instance.GetPlayer().isHandsfull?
        Data.HandFullDisplayFuntionName : Data.DisplayFuntionName:"none";
    public void OnClick()
    {
        if (this.GetComponentInChildren<IInteractable>() != null)
        {
            this.GetComponentInChildren<IInteractable>().OnClick(GameDataDNDL.Instance.GetPlayer().isHandsfull);
        }
        else
        {
            CustomLogs.CC_Log("Empty Table", color: "cyan");
        }
    }
    //public bool isTableActive()
    //{
    //    if (isTableEmpty()) return true;
    //    this.GetComponentInChildren<IInteractable>();
    //}
    public void OnMouseHoverEnter()
    {
        //myMaterial = GetComponent<MeshRenderer>().sharedMaterials[0];
        GetComponent<MeshRenderer>().material = GameDataDNDL.Instance.Selected;
    }
    public void OnMouseHoverExit()
    {
        GetComponent<MeshRenderer>().material = myMaterial;
    }
    
    public void SetOnTable(GameObject prefab,string tablename,EquipmentType type)
    {
        equipmentType=type;
        TableName = tablename;
        Instantiate(prefab, ItemParent.transform);
        TableDataRefresh();
    }
}
