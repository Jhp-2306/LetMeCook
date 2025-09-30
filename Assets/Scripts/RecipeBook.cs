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
    public int GetDishCookTime(Dishes dish)
    {
        foreach (var recipe in _recipes)
        {
            if (recipe.GetCookTimeData(dish) != -1)
            {
                return recipe.GetCookTimeData(dish);
            }
        }
        return -1;
    }
}

