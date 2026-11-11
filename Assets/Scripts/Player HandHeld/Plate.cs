using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plate : MonoBehaviour, IHandHeld, IObjectPoolingSettings
{
    //public typeofhandheld type;
    [SerializeField]
    List<ProcedureStep> ingredients;
    [SerializeField]
    List<Recipes> AssumedDishRecipe;
    [SerializeField]
    Dishes FinalDish;
    public bool isPlateEmpty = true;
    float Price, PriceMultiplier;

    public bool isDishDone;
    public GameObject MergerVFX;
    public bool canCombine()
    {
        if (AssumedDishRecipe != null)
        {
            foreach (var t in AssumedDishRecipe)
            {
                if (t.IsthisDish(ingredients)) return true;
            }
        }
        return false;
    }
    public Dishes GetFinalDish()
    {
        return FinalDish;

    }

    public void AddEvent()
    {
        InputManager.Instance.Interactionbtn.ResetButton("Interact");
        return;
    }

    public object CustomEventReturner()
    {
        return null;
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public typeofhandheld IGetType()
    {
        return typeofhandheld.plate;
    }
    public bool tryMergeRecipes(ProcedureStep step)
    {
        if (ingredients == null) ingredients = new List<ProcedureStep>();
        if (ingredients.Count == 0)
        {
            //Add the ingredient 
            ingredients.Add(step);
            //Get All possible Recipes
            AssumedDishRecipe = GameDataDNDL.Instance.GetAllThePossibleDishes(ingredients);
            isPlateEmpty = false;
            return true;
        }
        else
        {
            //check the merge recipe
            var tempingredients = new List<ProcedureStep>(ingredients);
            tempingredients.Add(step);
            var templist = new List<Recipes>(AssumedDishRecipe);
            foreach (var recipes in AssumedDishRecipe)
            {
                if (!recipes.CanBeThisDish(tempingredients))
                {
                    templist.Remove(recipes);
                }
            }
            if (templist.Count > 0)
            {
                ingredients.Add(step);
                //merge here
                //if (AssumedDishRecipe.Count == 1&& ingredients.Count == AssumedDishRecipe[0].items.Count)// final Dish
                //{
                //    FinalDish = AssumedDishRecipe[0].OutputDish;
                //}
                AssumedDishRecipe = templist;
            }
            else//cant be merge
            {
                //Toast here Cant be merge
                HUDManagerDNDL.Instance.ShowToastMsg("Cant Be Merge");
                return false;
            }
            isPlateEmpty = false;
            return true;

        }
        //return false;
    }

    public float GetFinalPrice()
    {
        return Price + (Price * (PriceMultiplier / 10));
    }
    public float GetPriceMultiplier()
    {
        return PriceMultiplier / 10;
    }
    public void pickupDish(Dishes dish, float price, float multi)
    {
        FinalDish = dish;
        isDishDone = true;
        Price = price;
        PriceMultiplier = multi;
        GiveProficiencyXp(PriceMultiplier);
        this.gameObject.GetComponent<Dish>().SetaDishActive(dish);
        isPlateEmpty = false;
    }
    public float RateTheDish()
    {
        if (PriceMultiplier <= 1.5 && PriceMultiplier > 1.25)
        {
            return 1f;
        }
        if (PriceMultiplier <= 1.25)
        {
            return 0.5f;
        }
        return 0f;
    }
    public void ResetPlate()
    {
        FinalDish = Dishes.none;
        isDishDone = false;
        Price = 0;
        PriceMultiplier = 0;
        ingredients.Clear();
        //Visual Set them Off here
    }
    public void MergeTheDish()
    {
        if (AssumedDishRecipe != null && AssumedDishRecipe.Count > 1)
        {
            foreach (var t in AssumedDishRecipe)
            {
                if (t.IsthisDish(ingredients))
                {
                    FinalDish = t.OutputDish;
                    isDishDone = true;
                    Price = t.price;
                    PriceMultiplier = t.CalculatePriceBonus(ingredients);
                    GiveProficiencyXp(PriceMultiplier);
                    return;
                }
            }
            return;
        }
        FinalDish = AssumedDishRecipe[0].OutputDish;
        isDishDone = true;
        Price = AssumedDishRecipe[0].price;
        PriceMultiplier = AssumedDishRecipe[0].CalculatePriceBonus(ingredients);
        GiveProficiencyXp(PriceMultiplier);


    }
    private void GiveProficiencyXp(float multi)
    {
        if (multi <= 1.5 && multi > 1.25)
        {
            //max XP 25
            ProficiencySystem.Instance.SetDishXP(FinalDish, 25);
            Debug.Log(CustomLogs.CC_TagLog("Plate", $"Price{Price}, Multi{PriceMultiplier}, Dish{FinalDish}, ProXP +25"));
        }
        if (multi <= 1.25 && multi > 1)
        {
            //mid XP 15
            ProficiencySystem.Instance.SetDishXP(FinalDish, 15);
            Debug.Log(CustomLogs.CC_TagLog("Plate", $"Price{Price}, Multi{PriceMultiplier}, Dish{FinalDish}, ProXP +15"));


        }
        if (multi <= 1)
        {
            ProficiencySystem.Instance.SetDishXP(FinalDish, 10);
            Debug.Log(CustomLogs.CC_TagLog("Plate", $"Price{Price}, Multi{PriceMultiplier}, Dish{FinalDish}, ProXP +10"));
            //min 10
        }
        // Debug.Log(CustomLogs.CC_TagLog("Plate", $"Price{Price}, Multi{PriceMultiplier}, Dish{FinalDish}"));
            Debug.Log(CustomLogs.CC_TagLog("Plate", $"Price{Price}, Multi{PriceMultiplier}, Dish{FinalDish}, ProXP 0"));
    }
    //public int RatetheFood()
    //{
    //    if (PriceMultiplier <= 1.5 && PriceMultiplier > 1.25)
    //    {            
    //        return 1;
    //    }
    //    if (PriceMultiplier <= 1.25 && PriceMultiplier > 1)
    //    {           
    //        return 2;
    //    }
    //    if (PriceMultiplier <= 1)
    //    {            
    //        return 3;
    //    }
    //    return 0;
    //}

    public void OPSetting_Reset()
    {
        //throw new System.NotImplementedException();
        ResetPlate();
        this.gameObject.SetActive(false);
    }
   
}
