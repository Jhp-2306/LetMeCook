using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [System.Serializable]
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
        None = 0,
        Chopped = 1,
        FineChopped = 2,
        Boiled = 3,
        Fryed = 4,
        cooked = 5,
        Burned = 6,
    }

    public enum Dishes
    {
        dish01,
        dish02,
        dish03,
        dish04,
        dish05,
        trash,
        count,
    }

    public enum DishCoookingStatus
    {
        None = 0,
        UnderCooked=1,
        Cooked=2,
        OverCooked=3,
        Burned=4,
    }
    [System.Serializable]
    public struct ProcedureStep
    {
        public IngredientType Ingredient;
        public processIngredient processed;
        public string Descriptions;

    }
    [System.Serializable]
    public class Recipes
    {
        public List<ProcedureStep> items;
        public Dishes OutputDish;
        public int CookingTime;
        public bool IsthisDish(List<ProcedureStep> _items)
        {
            //foreach (ProcedureStep item in _items) { 
            //if(!items.Contains(item)) return false;
            //}
            //return true;
            foreach (var item in _items)
            {
                if (!GetthisDishIngreident().Contains(item.Ingredient))
                {
                    return false;
                }
                if(!isIngreidentProcessedcorrectly(item))return false;
            }
            return true;
        }
        public int GetCookTimeData(Dishes dish)
        {
            if(dish==OutputDish) return CookingTime;
            else return -1;
        }
        List<IngredientType> GetthisDishIngreident()
        {
            List<IngredientType> _ret = new List<IngredientType>();
            foreach (var item in items)
            {
                _ret.Add(item.Ingredient);
            }
            return _ret;
        }
        bool isIngreidentProcessedcorrectly(ProcedureStep _step)
        {
            foreach (var item in items)
            {
                if (item.Ingredient.Equals(_step.Ingredient))
                {
                    //Finechopping and chopping are same thing but a different Result 
                    if (item.processed == processIngredient.Chopped && (_step.processed == processIngredient.Chopped || _step.processed == processIngredient.FineChopped))
                        return true;
                    if (item.processed == _step.processed) return true;

                }
            }
            return false;
        }
    }
    [System.Serializable]
    public class SaveDataType<T> where T : SaveDataTemplate
    {
        public Dictionary<string, T> Data;
        public SaveDataType()
        {
            Data = new Dictionary<string, T>();
        }
        public void AddOrUpdate(T data)
        {
            if (Data.ContainsKey(data.id))
            {
                Debug.Log(CustomLogs.CC_TagLog("SaveSystem", $"Updating ID:{data.id}"));
                Data[data.id] = data;
            }
            else
            {
                Debug.Log(CustomLogs.CC_TagLog("SaveSystem", $"Adding New ID:{data.id}"));
                Data.Add(data.id, data);
            }
        }
        public List<T> GetAllTheData() => Data.Values.ToList();

    }
    [System.Serializable]
    public class SaveDataTemplate
    {
        public string id;
        public Vector3 Position;
        public Quaternion Rotation;

        public object Data;
        public EquipmentType Type;

        public string GetId()
        {
            return id;
        }
    }

    public enum PlayableAreas
    {
        None,
        Kitchen,
        Room,
    }
}
