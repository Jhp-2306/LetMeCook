using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionBTN : MonoBehaviour
{
    public TextMeshProUGUI ButtonText;
    Button Interactionbtn;
    const float TAU = 6.283185307179586f;

    [Space(10f)]
    [Header("shop Contents")]
    public int segment = 10;
    [Range(0f,300f)]
    public float radius = 5f;
    [Range(0, 360f)]
    public float angleofmovement = 0;
    [Range(0, 360f)]
    public float angleofoffset = 0;
    public List<GameObject> ItemHolder;
    public int VisibleItem=7;
    public List<GameObject> Debuffer;
    float[] m_itemAngle;
    [Header("Shop")]
    [SerializeField]GameObject ShopParent;
    [SerializeField]GameObject ShopUIParent;
    bool isShopOpen;
    [SerializeField]Slider ShopScroller;
    [Header("Inventory")]
    [SerializeField] GameObject InventoryUIObj;
    [SerializeField] InventoryUI inventoryUI;
    bool isInventoryOpen;
   
    private void Awake()
    {
        Interactionbtn = this.gameObject.GetComponent<Button>();
    }
    private void OnDisable()
    {
        InputManager.OnDrag -= onValuChange;
    }
    public void AddEvent(Table table, string btnname, bool isbuttonActive,bool isTableEmpty)
    {
        if (!isbuttonActive)
        {
            ShopManager.Instance.CurrentSelectTable = null;
            Interactionbtn.gameObject.SetActive(isbuttonActive);
            Interactionbtn.onClick.RemoveAllListeners();
            return;
        }
        if (isTableEmpty)
        {
            ShopManager.Instance.CurrentSelectTable = table;
            Interactionbtn.gameObject.SetActive(true);
            ButtonText.text = "Buy";
            Interactionbtn.onClick.RemoveAllListeners();
            Interactionbtn.onClick.AddListener(delegate { SetupShop(); });
        }
        else
        {
        Interactionbtn.gameObject.SetActive(true);
        ButtonText.text = btnname;
        Interactionbtn.onClick.RemoveAllListeners();
        Interactionbtn.onClick.AddListener(delegate { table.OnClick(); });
        }
    }
    public void AddEvent(string btnname,Action OnClick)
    {
        Interactionbtn.gameObject.SetActive(true);
        ButtonText.text = btnname;
        Interactionbtn.onClick.RemoveAllListeners();
        Interactionbtn.onClick.AddListener(delegate { OnClick(); });
    }
    public void ResetButton(string btnname="Interact")
    {
        Interactionbtn.gameObject.SetActive(false);
        ButtonText.text = btnname;
        Interactionbtn.onClick.RemoveAllListeners();
    }

    //private void OnDrawGizmos()
    //{
    //    var degree = 360 / segment;
    //    var currentpos = this.gameObject.GetComponent<RectTransform>().transform.position;
    //    for (int i = 0; i <= segment; i++)
    //    {
    //        //var x = (Mathf.Sin((((degree*i) / 360) * TAU) + (angleofoffset / 360) * TAU) * radius);
    //        //var y = (Mathf.Cos((((degree * i) / 360) * TAU) + (angleofoffset / 360) * TAU) * radius);
    //        var vec = GetpositionFortheCircle(currentpos, (degree * i));
    //        //CustomLogs.CC_Log($"{degree * i}+{x},{y}", "cyan");
    //        Gizmos.DrawSphere(vec, 10f);
    //    }
    //    degree = 360 / (VisibleItem);
    //    for (int i = 0; i < VisibleItem; i++)
    //    {
    //        var vec = GetpositionFortheCircle(currentpos, (degree * i));
    //        ItemHolder[i].transform.position = vec;
    //    }
    //    var xcord = (Mathf.Sin(((angleofmovement / 360) * TAU) + (angleofoffset / 360) * TAU) * radius);
    //    var ycord = (Mathf.Cos(((angleofmovement / 360) * TAU) + (angleofoffset / 360) * TAU) * radius);
    //    Gizmos.DrawSphere(new Vector3(currentpos.x + xcord, currentpos.y + ycord, 0), 20f);
    //}

    #region Shop Functions
    public Vector3 GetpositionFortheCircle(Vector3 offset,float angle)
    {
        var xcord = (Mathf.Sin(((angle / 360) * TAU) + (angleofoffset / 360) * TAU) * radius);
        var ycord = (Mathf.Cos(((angle / 360) * TAU) + (angleofoffset / 360) * TAU) * radius);
        return new Vector3(offset.x + xcord, offset.y + ycord, 0);
    }
    public void SetupShop()
    {
        InputManager.OnDrag += onValuChange;
        if (isShopOpen) return;
        CustomLogs.CC_Log("Setting up Shop", "cyan");
        isShopOpen = true;
        ButtonText.gameObject.SetActive(!isShopOpen);
        StartCoroutine(ShopScrollerUpdate());
    }
    IEnumerator ShopScrollerUpdate()
    {
        ShopParent.SetActive(true);
        var templist = ShopManager.Instance.GetItems();
        VisibleItem = VisibleItem > ShopManager.Instance.MaxShopItems ? ShopManager.Instance.MaxShopItems : VisibleItem;
        var degree = 360 / (VisibleItem);
        var currentpos = this.gameObject.GetComponent<RectTransform>().transform.position;
        while (radius < 300)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < VisibleItem; i++)
            {
                var vec = GetpositionFortheCircle(currentpos, (degree * i));
                ItemHolder[i].transform.position = vec;
                //Debug.Log($"Cehcek {templist[i].GetPrefab() == null}");

                ItemHolder[i].GetComponent<ShopItemHolderUI>().Setup(templist[i].GetName(), templist[i].GetPrice(), templist[i].GetPrefab(), templist[i].GetEquipmentType()) ;
            }
            radius+=10;
        }
        ShopUIParent.SetActive(true);
    }
    
    
    float currentAngle;
    int m_maxItem;
    public void onValuChange(float val)
    {
        var templist = ShopManager.Instance.GetItems();
        m_maxItem = templist.Count-1;
        var degree = 360 / (VisibleItem);
        var currentpos = this.gameObject.GetComponent<RectTransform>().transform.position;
        //CustomLogs.CC_Log($"{(m_maxItem) * degree}","cyan");
        currentAngle += val;
        currentAngle = Mathf.Clamp(currentAngle, 0, (m_maxItem-1)*degree);
        for (int i = 0; i < VisibleItem; i++)
        {
            var vec = GetpositionFortheCircle(currentpos, currentAngle + (degree * i));
            ItemHolder[i].transform.position = vec;
            //Debug.Log($"Cehcek {templist[i].GetPrefab() == null}");
            ItemHolder[i].GetComponent<ShopItemHolderUI>().Setup(templist[i].GetName(), templist[i].GetPrice(), templist[i].GetPrefab(), templist[i].GetEquipmentType());
        }
    }
    public void ShopClose()
    {
        ShopManager.Instance.CurrentSelectTable = null;
        InputManager.OnDrag -= onValuChange;
        radius = 0f;
        var templist = ShopManager.Instance.GetItems();
        var degree = 360 / (VisibleItem);
        var currentpos = this.gameObject.GetComponent<RectTransform>().transform.position;
        for (int i = 0; i < VisibleItem; i++)
        {
            var vec = GetpositionFortheCircle(currentpos, (degree * i));
            ItemHolder[i].transform.position = vec;
            ItemHolder[i].GetComponent<ShopItemHolderUI>().Setup(templist[i].GetName(), templist[i].GetPrice(), templist[i].GetPrefab(), templist[i].GetEquipmentType());
        }
        ShopUIParent.SetActive(false);
        ShopParent.SetActive(false);
        isShopOpen = false;
        ButtonText.gameObject.SetActive(!isShopOpen);
    }
    #endregion

    #region Inventory System

    public void OpenInventory(BasicStorageSystem<IStorageItem> items)
    {
        if(isInventoryOpen) return;
        isInventoryOpen = true;
        InventoryUIObj.SetActive(true);
        inventoryUI.Set(items);
    }
    public void CloseInventory()
    {
        if (!isInventoryOpen) return;
        isInventoryOpen = false;
        InventoryUIObj.SetActive(false);
    }

    #endregion

    #region ProgressBarr
    [Header("Progess Bar Details")]
    [SerializeField] Image Fill;
    [SerializeField] TextMeshProUGUI ProgressValueTXT;
    [SerializeField] GameObject ProgressParent;


    public void SetProgressBar(float progress)
    {
        Fill.fillAmount = progress;
        ProgressValueTXT.text=(progress*100).ToString("f1");
        ProgressParent.SetActive(true);
    }

    public void OnProgressDone()
    {
        ProgressParent.SetActive(false);
    }

    public IEnumerator TimerProgressBar(float maxTimer, Action Callback)
    {
        var timer = 0f;
        while (timer <= maxTimer)
        {
            yield return null;
            timer += Time.deltaTime;
            SetProgressBar(timer / maxTimer);
            if (timer > maxTimer)
                OnProgressDone();
        }
        Callback();
    }

    #endregion 
}
