using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plate : MonoBehaviour, IHandHeld,IObjectPoolingSettings
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

    public bool canCombine()
    {
        if(AssumedDishRecipe != null)
        {
            foreach (var t in AssumedDishRecipe)
            {
                if (t.IsthisDish(ingredients))return true;
            }
        }
        return false;
    }
    public Dishes GetFinalDish()
    {
        //if (AssumedDishRecipe != null && AssumedDishRecipe.Count > 1)
        //{
        //    foreach (var t in AssumedDishRecipe)
        //    {
        //        if (t.IsthisDish(ingredients))
        //            return t.OutputDish;
        //    }
        //    return Dishes.trash;
        //}
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
        return PriceMultiplier/10;
    }
    public void pickupDish(Dishes dish, float price, float multi)
    {
        FinalDish = dish;
        isDishDone = true;
        Price = price; 
        PriceMultiplier = multi;
        this.gameObject.GetComponent<Dish>().SetaDishActive(dish);
        isPlateEmpty = false;
    }
    public void RateTheDish()
    {

    }
    public void ResetPlate()
    {
        FinalDish = Dishes.none;
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
                    FinalDish= t.OutputDish;
                    isDishDone = true;
                    return;
                }
            }
            return;
        }
        FinalDish = AssumedDishRecipe[0].OutputDish;
        isDishDone = true;


    }

    public void OPSetting_Reset()
    {
        //throw new System.NotImplementedException();
        ResetPlate();
        this.gameObject.SetActive(false);
    }
}
