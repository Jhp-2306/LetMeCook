using Constants;
using HandHeld;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using static UnityEditor.Progress;

public class Refrigerator : Table, IInteractable
{
    BasicStorageSystem<IStorageItem> storageSystem;

    private void Awake()
    {
        base.Awake();
        Init();
    }
    void Init()
    {
        if (storageSystem == null)
        {
            storageSystem = new BasicStorageSystem<IStorageItem>(4);
        }
        //storageSystem.AddItems(new RefrigeratorItems(IngredientType.Tomato, 10));
        //storageSystem.AddItems(new RefrigeratorItems(IngredientType.Apple, 2));
        //storageSystem.AddItems(new RefrigeratorItems(IngredientType.Mango, 7));
        Data = this.GetEquipmentData();
        equipmentType = EquipmentType.Refrigerator;
        //CustomLogs.CC_Log($"Setting Refrigerator {Data.DisplayFuntionName},{Data.name}", "red");
    }
    public SO_EquipmentData GetEquipmentData()
    {
        var t = new SO_EquipmentData();
        t.name = "Refrigerator";
        t.DisplayFuntionName = "Open";
        t.HandFullDisplayFuntionName = "Add";
        t.Functionatily = this.gameObject;
        return t;
    }

    public EquipmentType GetEquipmentType()
    {
        return EquipmentType.none;
    }

    public void OnClick(bool ishandfull = false)
    {

        if (ishandfull && GameDataDNDL.Instance.GetPlayer().InHand.IGetType() == typeofhandheld.box)
        {
            CustomLogs.CC_Log("Starting Adding items Box -> inventory", "red");
            //ToDo: Stop Player input have 1-3s time for thing to get added into the refrigerator
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
                               //create a new item slot when there is know item in the storage
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
               CustomLogs.CC_Log("Done Adding items Box -> inventory", "red");
           });

        }
        else
        {
            CustomLogs.CC_Log("Opening inventory", "red");
            HUDManagerDNDL.Instance.OpenInventory(storageSystem);
        }


    }
    //IEnumerator TimerProgressBar(int maxTimer, Action Callback)
    //{
    //    var timer = 0f;
    //    while (timer <= maxTimer)
    //    {
    //        yield return null;
    //        timer += Time.deltaTime;
    //        HUDManagerDNDL.Instance.SetProgressBar(timer / maxTimer);
    //        if (timer > maxTimer)
    //            HUDManagerDNDL.Instance.ProgressBarDone();
    //    }
    //    Callback();
    //}

    public bool IsInteractable()
    {
        //if (!GameDataDNDL.Instance.GetPlayer().isPlayerHandEmpty)
        //{
        //    //if ((GameDataDNDL.Instance.GetPlayer().InHand.IGetType() != typeofhandheld.box || !GameDataDNDL.Instance.GetPlayer().isHandsfull))
        //    return true;
        //}
        //else
        return true;
        //return false;
    }
}
public class RefrigeratorItems : IStorageItem
{
    IngredientType type;
    int count;
    int slotCapcity = 20;

    public RefrigeratorItems(IngredientType _type, int _count)
    {
        this.type = _type;
        this.count = _count;
        //this.slotCapcity = _slotCapcity;
    }

    public IngredientType Type => type;

    public void AddQuanitity(int quanitity, out int remaining)
    {
        if (count + quanitity <= slotCapcity)
        {
            count += quanitity;
            remaining = 0;
        }
        else
        {
            remaining = (count + quanitity) - slotCapcity;
            count = slotCapcity;
        }
    }

    public int GetQuanitity() => count;


    public bool isMaxed() => count == slotCapcity;


    public void RemoveQuanitity()
    {
        if (count > 0)
        {
            count--;
        }

    }
}
