using Constants;
using HandHeld;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;



public class Refrigerator : InteractiveBlock
{
    #region Data Saving
    
    public override void ReadFromSave(SaveDataTemplate _data)
    {
       base.ReadFromSave(_data);
        var data= BasicStorageSystem<IStorageItem>.LoadDataFromString(_data.Data.ToString());
        //Slots =BasicStorageSystem<IStorageItem>.LoadSizeFromString(_data.Data.ToString());
        storageSystem=new BasicStorageSystem<IStorageItem>(Slots);
        Debug.Log(data.Count);
        foreach (var slot in data)
        {
            Debug.Log(slot);
            RefrigeratorItems loadedItem = new RefrigeratorItems(slot);
            storageSystem.AddItems(loadedItem);
        }
        //Data = this.GetEquipmentData();
        equipmentType = EquipmentType.Refrigerator;
        //BeforeSaving();
        LoadLevelUpgrades();
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
    }
    
    #endregion

    BasicStorageSystem<IStorageItem> storageSystem;
    int Slots=4;

    private void Awake()
    {
        
        
    }
    private void OnDisable()
    {
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
    }
    public void OnDestroy()
    {
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
    }
    public override void Init(EquipmentType _equip = EquipmentType.none, string item = "")
    {
        base.Init(EquipmentType.Refrigerator,item);
        if (storageSystem == null)
        {
            storageSystem = new BasicStorageSystem<IStorageItem>(Slots);
        }
        savedata.Data = storageSystem.GetAllDataInString();
        //Data = this.GetEquipmentData();
        equipmentType = EquipmentType.Refrigerator;
        LoadLevelUpgrades();
        BeforeSaving();
        GameSaveDNDL.DataUpdateBeforeSave -= BeforeSaving;
        GameSaveDNDL.DataUpdateBeforeSave += BeforeSaving;
    }
    void BeforeSaving()
    {
        savedata.Data = storageSystem.GetAllDataInString();
        GameSaveDNDL.Instance.AddSaveData(savedata);
    }
    
    public EquipmentType GetEquipmentType()
    {
        return EquipmentType.none;
    }
    public void AddFTUTIngredient()
    {
        storageSystem.AddItems(new RefrigeratorItems(IngredientType.Tomato, 2));
    }
    public override void OnClick()
    {
        
        var _player = GameDataDNDL.Instance.GetPlayer();
        if (_player.InHand!=null && _player.InHand.IGetType() == typeofhandheld.box)
        {
            CustomLogs.CC_Log("Starting Adding items Box -> inventory", "red");
            //Stop Player input have 1-3s time for thing to get added into the refrigerator
            //Coroutine time = StartCoroutine(TimerProgressBar
            HUDManagerDNDL.Instance.SetProgressBar(3,
                () =>

           {
               //StopCoroutine(time);
               //Additem
               if (GameDataDNDL.Instance.GetPlayer().InHand.IGetType() == typeofhandheld.box)
               {
                   var box = (DeliveryBox)GameDataDNDL.Instance.GetPlayer().InHand;
                   var item = box.CustomEventReturner();
                   bool canDestory = true;
                   List<RefrigeratorItems> remainingItems = new List<RefrigeratorItems>();
                   while (item != null)
                   {
                       //var temp =temp1;
                       if (item != null)
                       {
                           CustomLogs.CC_Log($"Total item in the Box {item.GetQuanitity()}", "cyan");
                           if (storageSystem.GetAllItems().Count > 0)
                           {
                               int remain = 0;
                               bool isAdded = false;
                               foreach (var initem in storageSystem.GetAllItems())
                               {
                                   if (initem.Type == item.Type)
                                   {
                                       //Add item if there are already there
                                       initem.AddQuanitity(item.GetQuanitity(), out remain);
                                       isAdded = true;
                                   }
                               }
                               // create a new item slot if there is no item in the storage 
                               if (!isAdded)
                               {
                                   canDestory = storageSystem.AddItems(new RefrigeratorItems(item.Type, item.GetQuanitity()));
                               }
                               // create a new item slot for the remain item in the box
                               if (remain > 0)
                               {
                                   canDestory = storageSystem.AddItems(new RefrigeratorItems(item.Type, remain));
                                   if (!canDestory)
                                       remainingItems.Add(new RefrigeratorItems(item.Type, remain));
                                   //box.AddBoxItem(new RefrigeratorItems(item.Type, remain));
                               }

                           }
                           else
                           {
                               //create a new item slot when there is no item in the storage
                               {
                                   storageSystem.AddItems(new RefrigeratorItems(item.Type, item.GetQuanitity()));
                               }
                           }

                       }
                       else
                       {
                           CustomLogs.CC_Log("null temp", "red");
                       }
                       //if (!canDestory) break;
                       item = (IStorageItem)box.CustomEventReturner();
                   }

                   //Removing the Box from the Hand
                   if (canDestory)
                   {
                       GameDataDNDL.Instance.GetPlayer().RemoveFromHand();

                   }
                   else //add remaining items back to the box
                   {
                       HUDManagerDNDL.Instance.ShowToastMsg("Storage Full!!");
                       foreach (var remainingite in remainingItems)
                       {

                           box.AddBoxItem(remainingite);
                       }
                   }
               }
               else
               {
                   CustomLogs.CC_Log("null", "red");
               }
               savedata.Data = storageSystem.GetAllDataInString();
               CustomLogs.CC_Log("Done Adding items Box -> inventory", "red");
           });

        }
        else
        {
            CustomLogs.CC_Log("Opening inventory", "red");
            HUDManagerDNDL.Instance.OpenInventory(storageSystem);
        }
    }
    
    public override bool IsInteractionSatisfied()
    {
        return true;
    }
   
    public void LoadLevelUpgrades()
    {
       var temp= Data.upgradeData[savedata.level];
        Slots=(int)temp.GetUpgradeValue(E_ValueName.Slots);
    }
}
public class RefrigeratorItems : IStorageItem
{
    struct RefrigeratorSlotsData
    {
        public IngredientType type;
        public int count;
    }
    private RefrigeratorSlotsData _data;
    private const int slotCapcity = 20;

    public RefrigeratorItems(IngredientType _type, int _count)
    {
        this._data.type = _type;
        this._data.count = _count;
        //this.slotCapcity = _slotCapcity;
    }
    public RefrigeratorItems(string data)
    {
        var tempValue = JsonUtility.FromJson<RefrigeratorSlotsData>(data);
        _data.type = tempValue.type;
        _data.count = tempValue.count;
    }
    public IngredientType Type => _data.type;

    public void AddQuanitity(int quanitity, out int remaining)
    {
        if (_data.count + quanitity <= slotCapcity)
        {
            _data.count += quanitity;
            remaining = 0;
        }
        else
        {
            remaining = (_data.count + quanitity) - slotCapcity;
            _data.count = slotCapcity;
        }
    }

    public int GetQuanitity() => _data.count;


    public bool isMaxed() => _data.count == slotCapcity;


    public void RemoveQuanitity()
    {
        if (_data.count > 0)
        {
            _data.count--;
        }

    }
    public string GetData()
    {
        string data= JsonUtility.ToJson(_data); 
        return data;

    }
   

}
