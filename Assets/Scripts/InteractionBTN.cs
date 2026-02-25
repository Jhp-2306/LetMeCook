using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionBTN : MonoBehaviour
{
    public TextMeshProUGUI ButtonText;
    public TextMeshProUGUI HoldButtonText;
    Button Interactionbtn;
    EventTrigger trigger;
    const float TAU = 6.283185307179586f;

    //[Space(10f)]
    //[Header("shop Contents")]
    //public int segment = 10;
    //[Range(0f,300f)]
    //public float radius = 5f;
    //[Range(0, 360f)]
    //public float angleofmovement = 0;
    //[Range(0, 360f)]
    //public float angleofoffset = 0;
    //public List<GameObject> ItemHolder;
    //public int VisibleItem=7;
    //public List<GameObject> Debuffer;
    //float[] m_itemAngle;

    //[SerializeField]Slider ShopScroller;
    [Header("Inventory")]
    [SerializeField] GameObject InventoryUIObj;
    [SerializeField] InventoryUI inventoryUI;
    event Action OnClick;
    event Action OnHold;
    bool isButtonActionPerformed;
    bool isInventoryOpen;

    [SerializeField] Image purplebtn, cyanbtn, btnmask;
    
    private void Awake()
    {
        Interactionbtn = this.gameObject.GetComponent<Button>();
        trigger = this.GetComponent<EventTrigger>();
        ResetButton();
    }
    private void OnDisable()
    {
        //InputManager.OnDrag -= onValuChange;
    }
    public void AddEventsspc(Action _OnClick=null, Action _OnHold = null)
    {
        OnClick += _OnClick;
        OnHold += _OnHold;
    }
    public void AddEvent(InteractiveBlock table, string btnname, bool isbuttonActive)
    {
        {
            ////Disable the interaction button
            //if (!isbuttonActive)
            //{
            //    ShopManager.Instance.CurrentSelectTable = null;
            //    Interactionbtn.gameObject.SetActive(isbuttonActive);
            //    Interactionbtn.onClick.RemoveAllListeners();
            //    return;
            //}
            ////shop data
            //if (isTableEmpty)
            //{
            //    ShopManager.Instance.CurrentSelectTable = table;
            //    Interactionbtn.gameObject.SetActive(true);
            //    ButtonText.text = "Buy";
            //    Interactionbtn.onClick.RemoveAllListeners();
            //    Interactionbtn.onClick.AddListener(delegate { SetupShop(); });
            //}
            //else
        }
        //Adding the event
        Interactionbtn.gameObject.SetActive(true);
        ButtonText.text = btnname;
        HoldInfo.gameObject.SetActive(false); btnmask.gameObject.SetActive(false);
        if (!table.GetHoldInteractableName().Equals("none"))
        {
            HoldButtonText.text =table.GetHoldInteractableName();
            HoldInfo.gameObject.SetActive(true);
            btnmask.gameObject.SetActive(true);
        }
        OnClick = delegate { };
        OnHold = delegate { };
        OnClick += table.OnClick;
        OnHold += table.OnHold;
    }
    public void AddEvent(string btnname, Action _OnClick = null, Action _OnHold = null)
    {
        Interactionbtn.gameObject.SetActive(true);
        ButtonText.text = btnname;
        OnClick = delegate { };
        OnHold = delegate { };
        OnClick += _OnClick;
        OnHold += _OnHold;
    }
    public void ResetButton(string btnname = "Interact")
    {
        HoldInfo.gameObject.SetActive(false);
        Interactionbtn.gameObject.SetActive(false);

        ButtonText.text = btnname;
        OnClick = delegate { };
        OnHold = delegate { };
    }
   


    #region Old Shop Methods 
    //public Vector3 GetpositionFortheCircle(Vector3 offset,float angle)
    //{
    //    var xcord = (Mathf.Sin(((angle / 360) * TAU) + (angleofoffset / 360) * TAU) * radius);
    //    var ycord = (Mathf.Cos(((angle / 360) * TAU) + (angleofoffset / 360) * TAU) * radius);
    //    return new Vector3(offset.x + xcord, offset.y + ycord, 0);
    //}
    //public void SetupShop()
    //{
    //    InputManager.OnDrag += onValuChange;
    //    if (isShopOpen) return;
    //    CustomLogs.CC_Log("Setting up Shop", "cyan");
    //    isShopOpen = true;
    //    ButtonText.gameObject.SetActive(!isShopOpen);
    //    StartCoroutine(ShopScrollerUpdate());
    //}
    //IEnumerator ShopScrollerUpdate()
    //{
    //    ShopParent.SetActive(true);
    //    var templist = ShopManager.Instance.GetItems();
    //    VisibleItem = VisibleItem > ShopManager.Instance.MaxShopItems ? ShopManager.Instance.MaxShopItems : VisibleItem;
    //    var degree = 360 / (VisibleItem);
    //    var currentpos = this.gameObject.GetComponent<RectTransform>().transform.position;
    //    while (radius < 300)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        for (int i = 0; i < VisibleItem; i++)
    //        {
    //            var vec = GetpositionFortheCircle(currentpos, (degree * i));
    //            ItemHolder[i].transform.position = vec;
    //            //Debug.Log($"Cehcek {templist[i].GetPrefab() == null}");

    //            ItemHolder[i].GetComponent<ShopItemHolderUI>().Setup(templist[i].GetName(), templist[i].GetPrice(), templist[i].GetPrefab(), templist[i].GetEquipmentType()) ;
    //        }
    //        radius+=10;
    //    }
    //    ShopUIParent.SetActive(true);
    //}


    //float currentAngle;
    //int m_maxItem;
    //public void onValuChange(float val)
    //{
    //    var templist = ShopManager.Instance.GetItems();
    //    m_maxItem = templist.Count-1;
    //    var degree = 360 / (VisibleItem);
    //    var currentpos = this.gameObject.GetComponent<RectTransform>().transform.position;
    //    //CustomLogs.CC_Log($"{(m_maxItem) * degree}","cyan");
    //    currentAngle += val;
    //    currentAngle = Mathf.Clamp(currentAngle, 0, (m_maxItem-1)*degree);
    //    for (int i = 0; i < VisibleItem; i++)
    //    {
    //        var vec = GetpositionFortheCircle(currentpos, currentAngle + (degree * i));
    //        ItemHolder[i].transform.position = vec;
    //        //Debug.Log($"Cehcek {templist[i].GetPrefab() == null}");
    //        ItemHolder[i].GetComponent<ShopItemHolderUI>().Setup(templist[i].GetName(), templist[i].GetPrice(), templist[i].GetPrefab(), templist[i].GetEquipmentType());
    //    }
    //}
    //public void ShopClose()
    //{
    //    ShopManager.Instance.CurrentSelectTable = null;
    //    InputManager.OnDrag -= onValuChange;
    //    radius = 0f;
    //    var templist = ShopManager.Instance.GetItems();
    //    var degree = 360 / (VisibleItem);
    //    var currentpos = this.gameObject.GetComponent<RectTransform>().transform.position;
    //    for (int i = 0; i < VisibleItem; i++)
    //    {
    //        var vec = GetpositionFortheCircle(currentpos, (degree * i));
    //        ItemHolder[i].transform.position = vec;
    //        ItemHolder[i].GetComponent<ShopItemHolderUI>().Setup(templist[i].GetName(), templist[i].GetPrice(), templist[i].GetPrefab(), templist[i].GetEquipmentType());
    //    }
    //    ShopUIParent.SetActive(false);
    //    ShopParent.SetActive(false);
    //    isShopOpen = false;
    //    ButtonText.gameObject.SetActive(!isShopOpen);
    //}
    #endregion

    #region Inventory System

    public void OpenInventory(BasicStorageSystem<IStorageItem> items)
    {
        if (isInventoryOpen) return;
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

    public Transform GetItemSlots()
    {
        return inventoryUI.GetItemSlots();
    }
    #endregion

    #region ProgressBarr
    [Header("Progess Bar Details")]
    [SerializeField] Image Fill;
    [SerializeField] TextMeshProUGUI ProgressValueTXT;
    [SerializeField] GameObject ProgressParent;

    Coroutine progressionBarCoroutine;
    Coroutine onHoldCoroutine;

    public void SetProgressBar(float progress)
    {
        Fill.fillAmount = progress;
        ProgressValueTXT.text = (progress * 100).ToString("f1");
        ProgressParent.SetActive(true);
    }

    public void OnProgressDone()
    {
        ProgressParent.SetActive(false);
    }
    public void TimerProgressBar(float maxTimer, Action Callback)
    {
        if (progressionBarCoroutine != null)
        {
            StopCoroutine(progressionBarCoroutine);
            progressionBarCoroutine = null;
        }
        progressionBarCoroutine = StartCoroutine(HoldTimer(maxTimer, SetProgressBar, () =>
        {
            OnProgressDone();
            Callback?.Invoke();
            progressionBarCoroutine = null;
        }));
    }
    public IEnumerator HoldTimer(float maxTimer, Action<float> update = null, Action Callback = null)
    {
        var timer = 0f;
        while (timer <= maxTimer)
        {
            yield return null;
            timer += Time.deltaTime;
            update?.Invoke(timer / maxTimer);
        }
        Callback?.Invoke();
    }

    #endregion

    #region Holding Down
    public Image HoldFiller;
    public GameObject HoldUI;
    public GameObject HoldInfo;
    public void OnPressDown()
    {
        Debug.Log(CustomLogs.CC_TagLog("Interactive Button", "Hold Stating."));
        isButtonActionPerformed = false;
        if (onHoldCoroutine != null)
        {
            StopCoroutine(onHoldCoroutine);
            onHoldCoroutine = null;
        }
        //var timer = 0f;
        onHoldCoroutine = StartCoroutine(HoldTimer(2.4f, (value) =>
        {
            if (value > .2f)
                HoldUI.SetActive(true);
            HoldFiller.fillAmount = value * 0.4f;
        }, OnHoldSuccessful));
    }
    public void OnPressUp()
    {
        if (isButtonActionPerformed) return;
        Debug.Log(CustomLogs.CC_TagLog("Interactive Button", "Hold Failed. Calling the Click Action"));
        isButtonActionPerformed = true;
        if (onHoldCoroutine != null)
        {
            StopCoroutine(onHoldCoroutine);
            onHoldCoroutine = null;
        }
        HoldUI.SetActive(false);
        OnClick?.Invoke();
    }
    public void InvokeClick()
    {
        OnClick?.Invoke();
    }
    public void OnHoldSuccessful()
    {
        if (isButtonActionPerformed) return;
        Debug.Log(CustomLogs.CC_TagLog("Interactive Button", "Hold is Success. Calling the Hold Action"));
        isButtonActionPerformed = true;
        HoldUI.SetActive(false);
        OnHold?.Invoke();
    }
    #endregion
}
