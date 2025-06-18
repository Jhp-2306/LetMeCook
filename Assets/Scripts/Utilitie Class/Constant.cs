using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Constants
{
    public static class Constant
    {
        public static readonly string DateTimeFormate = "MM/dd/yyyy HH:mm";

    }
    public enum KitchenState
    {
        idleKitchen = 0,
        OpenKitchen = 1,
        CloseKitchen = 2
    }
    public enum GameState
    {
        PauseGame = 0,
        PlayGame = 1,
        TimeSkip = 2
    }
    public enum EquipmentType
    {
        Stove,
        Oven,
        Cutting_Board,
        Mixing_bowl,
        Whisk,
        Frying_pan,
        Blender,
        Grater,
        Pizza_cutter,
        Grill_pan,
        Refrigerator,
        none
    }

    [System.Serializable]
    public enum IngredientType
    {
        Carrot,
        Onion,
        Potato,
        Tomato,
        Spinach,
        Lettuce,
        Cabbage,
        Mushroom,
        Apple,
        Banana,
        Strawberry,
        Lemon,
        Orange,
        Pineapple,
        Mango,
        Cherry,
        Blueberry,
        Grapes,
        Watermelon,
        Peach,
        Bread,
        Rice,
        Pasta,
        BurgerBun,
        Chicken,
        Beef,
        Pork,
        Bacon,
        FishSalmon,
        FishTuna,
        Shrimp,
        Egg,
        Meatballs,
        Milk,
        Cheese,
        Butter,
        count
    }
    [System.Serializable]
    public enum processIngredient
    {
        None=0,
        Chopped=1,
        FineChopped=2,
        Boiled=3 ,
        Fryed=4,
        cooked=5
    }

    public enum Dishes
    {
        dish01,
        dish02,
        dish03,
        dish04,
        dish05,
        count,
    }

    public class IngredientData
    {
        public IngredientType Type;
        public int Count;
    }
    public struct ProcedureStep
    {
        public IngredientData Ingredient;
        public processIngredient processed;
        public string Descriptions; 
    }

    public class RecipeBook
    {
        private static int NUMBER_OF_PROCEDURE_STEP = (int)IngredientType.count;
        public ProcedureStep CurrentStep;
        public RecipeBook[] subnode = new RecipeBook[NUMBER_OF_PROCEDURE_STEP];

        public RecipeBook(ProcedureStep currentStep)
        {
            CurrentStep = currentStep;
        } 
        
    }
}
