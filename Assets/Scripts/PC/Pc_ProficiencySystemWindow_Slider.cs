using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Constants;
using Unity.VisualScripting;
using System.Linq;

public class Pc_ProficiencySystemWindow_Slider : MonoBehaviour
{
    [System.Serializable]
    public struct ingredientDisplay
    {
        public TMPro.TextMeshProUGUI Count;
        public Image icon;
    }
    public Dishes dish;
    private string DishName;
    public bool isExpand;
    public bool isUnlocked;
    public TMPro.TextMeshProUGUI DishNameTXt;
    public TMPro.TextMeshProUGUI Level;
    public TMPro.TextMeshProUGUI Recipe;
    public List<ingredientDisplay> displayList;
    public TMPro.TextMeshProUGUI XP;
    public RectTransform Popout;
    public RectTransform Parent;

    public GameObject GreenPopoutButton,RedPopoutButton;

    [Space(10)]
    [Header("Animation Settings")]
    private Coroutine animationPopout;
    public AnimationCurve PopoutanimationCurve;
    public float animationPopoutTime = 0.75f;

    public float CollapsParentHeight = 280;
    public float ExpandParentHeight = 560;
    public float CollapsPopoutHeight = 0;
    public float ExpandPopoutHeight = 323;

    public GameObject Locked, Unlocked;
    public Material unlockedMaterial;
    void init()
    {
        var temp = ProficiencySystem.Instance.GetProficiencyStats(dish);
        DishNameTXt.text=dish.ToString();
        Level.text = $"Level {temp.GetLevel()}";
        XP.text = $"{temp.GetCurrentXp()}/{temp.GetXpMaxCap()}";
        Recipe.text= temp.recipe;
        for (int i = 0; i < displayList.Count; i++) {
            if (i < temp.Ingredients.Count)
            {
                bool isunlocker = GameDataDNDL.Instance.unlockedIngredient.Contains(temp.Ingredients.Keys.ToArray()[i]);
                displayList[i].icon.sprite = AssetLoader.Instance.GetIngredientIcon(temp.Ingredients.Keys.ToArray()[i]);
                displayList[i].icon.material = isunlocker ? null : AssetLoader.Instance.IconLockedMaterial;
                displayList[i].Count.text = $"X{temp.Ingredients.Values.ToArray()[i]}";
                displayList[i].Count.fontSharedMaterial = isunlocker ? unlockedMaterial:AssetLoader.Instance.TMPLockedMaterial;
            }
            else
            {
                displayList[i].Count.gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
        Unlocked.gameObject.SetActive(temp.isUnlocked());
        Locked.gameObject.SetActive(!temp.isUnlocked());
    }

    public void OnTap()
    {
        if (isUnlocked)
        {
            if(animationPopout != null)
                StopCoroutine(animationPopout);
            animationPopout=StartCoroutine(PlayPopoutAnimation());
            setButtonLabel(!isExpand);

        }
       
    }
    void setButtonLabel(bool _isExpand)
    {
        RedPopoutButton.SetActive(_isExpand);
        GreenPopoutButton.SetActive(!_isExpand);
       //PopoutButtonLabel.text = isExpand ? "Collaps" : "View Recipe";
    }
    private void Start()
    {
        //Force Collaps
        Popout.sizeDelta = new Vector2(Popout.rect.width, 0f);
        Parent.sizeDelta = new Vector2(Parent.rect.width, 280f);
        isExpand = false;
        setButtonLabel(isExpand);
        Debug.Log("here");
        

    }
    public void EventSub()
    {
        ProficiencySystem.OnInit -= init;
        ProficiencySystem.OnInit += init;
    }
    public void OnDestroy()
    {
    ProficiencySystem.OnInit -= init;
        
    }
    //Popout Animations
    IEnumerator PlayPopoutAnimation()
    {
        var time = 0f;
        var initPopout = Popout.sizeDelta.y;
        var initParent = Parent.sizeDelta.y;
        var difPopout = (isExpand ? CollapsPopoutHeight : ExpandPopoutHeight) - initPopout;
        var difParent = (isExpand ? CollapsParentHeight : ExpandParentHeight) -initParent;
        while (time <= animationPopoutTime)
        {
            time += Time.deltaTime;
            Popout.sizeDelta = new Vector2(Popout.rect.width, initPopout+(difPopout* PopoutanimationCurve.Evaluate(time/animationPopoutTime)));
            Parent.sizeDelta = new Vector2(Parent.rect.width, initParent + (difParent * PopoutanimationCurve.Evaluate(time / animationPopoutTime)));
            yield return null;

        }
        Popout.sizeDelta = new Vector2(Popout.rect.width, isExpand ? CollapsPopoutHeight : ExpandPopoutHeight);
        Parent.sizeDelta = new Vector2(Parent.rect.width, isExpand ? CollapsParentHeight : ExpandParentHeight);
        isExpand = !isExpand;
        animationPopout = null;
    }
}
