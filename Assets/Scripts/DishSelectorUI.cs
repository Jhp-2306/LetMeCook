using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Constants;
using System;
using UnityEngine.UI;

public class DishSelectorUI : MonoBehaviour
{
    public GameObject Parent;
    public List<GameObject> DishSlider;

    public List<GameObject> SelectedDishVisual;
    [SerializeField]
    private List<Dishes> Selected;
    public void Init()
    {
        var t = ProficiencySystem.Instance.getUnlockDish();
        Selected = new List<Dishes>();
        int idx = 0;
        foreach (GameObject slider in DishSlider)
        {
            if (t.Contains((Dishes)idx))
            {
                slider.name = ((Dishes)idx).ToString();
                slider.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = StringExtensions.CamelCaseToWords(((Dishes)idx).ToString());
                slider.GetComponent<Button>().interactable = true;
            }
            else
            {
                slider.name = ((Dishes)idx).ToString();
                slider.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "????";
                slider.GetComponent<Button>().interactable = false;
            }
            idx++;
        }
        foreach (GameObject slider in SelectedDishVisual) slider.SetActive(false);
        this.gameObject.SetActive(true);

    }
    public void OnClick(GameObject Btn)
    {
        if (Selected != null)
        {
            if (Selected.Contains(GetDishesFromString(Btn.name)))
            {
                //Deselect and return
                Selected.Remove(GetDishesFromString(Btn.name));
                foreach (var temp in SelectedDishVisual)
                {
                    if (temp.name == Btn.name)
                    {
                        temp.SetActive(false);
                    }
                    updateSelectedSlider();
                    return;
                }
            }
            //Select
            if (Selected.Count < 5)
            {
                Selected.Add(GetDishesFromString(Btn.name));
                updateSelectedSlider();
            }
        }
    }
    void updateSelectedSlider()
    {
        int idx = 0;
        foreach (var slider in SelectedDishVisual)
        {
            if (idx < Selected.Count)
            {
                slider.name = Selected[idx].ToString();
                slider.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = StringExtensions.CamelCaseToWords(Selected[idx].ToString());
                slider.SetActive(true);
            }
            else
            {
                slider.SetActive(false);
            }
            idx++;
        }
    }

    Dishes GetDishesFromString(string str)
    {
        for (int i = 0; i < (int)Dishes.count; i++)
        {
            if (String.Equals(str, ((Dishes)i).ToString()))
            {
                return (Dishes)i;
            }
        }
        return Dishes.trash;
    }

    public void StartTheDay()
    {
        //Start the Day
        GameDataDNDL.Instance.StartTheDay(Selected);
        this.gameObject.SetActive(false);
    }
}
