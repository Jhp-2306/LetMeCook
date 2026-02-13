using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeBook
{
    public SO_RecipeBook _RecipeBook;
    [SerializeField] List<Recipes> _recipes;

    public void init()
    {
        _recipes = new List<Recipes>();
        _recipes = _RecipeBook.items;
    }
    public Dishes GetDishes(List<ProcedureStep> steps)
    {
        //_recipes = _RecipeBook.items;
        foreach (Recipes recipe in _recipes)
        {
            if (recipe.IsthisDish(steps))
                return recipe.OutputDish;
        }
        return Dishes.trash;
    }
    public List<Recipes> GetAllThePossibleDishes(List<ProcedureStep> steps)
    {
        var dish=new List<Recipes>();
        foreach(Recipes recipe in _recipes)
        {
            //var t= ;
            if(recipe.CanBeThisDish(steps)) dish.Add(recipe);
        }
        return dish;

    }
    public float GetPrice(Dishes dish)
    {
        var _recipe = GetRecipe(dish);
        if (_recipe != null) return _recipe.price;
        return 0;
    }
    public float GetPriceMultipler(Dishes dish, List<ProcedureStep> steps)
    {
        var _recipe = GetRecipe(dish);
        if (_recipe != null)return _recipe.CalculatePriceBonus(steps);
        return 0;
    }
    public Recipes GetRecipe(Dishes dish)
    {
        foreach (var recipe in _recipes)
        {
            if (recipe.OutputDish == dish)
            {
                return recipe;
            }
        }
        return null;
    }
    public int GetDishCookTime(Dishes dish)
    {
        var _recipe=GetRecipe(dish);
        if( _recipe!=null )return _recipe.CookingTime;
        return -1;
    }

    public List<Recipes> GetRecipes() => _recipes;
}


