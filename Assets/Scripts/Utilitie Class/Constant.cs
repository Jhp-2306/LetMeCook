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
        //idleKitchen = 0,
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
        Table,
        TrashBin,
        PlateTray,
        Furnitures,
        none
    }

    [System.Serializable]
    public enum IngredientType
    {
        Carrot,
        Onion,
        Potato,
        Tomato,
        Lettuce,
        Mushroom,
        Bread,
        Rice,
        BurgerBun,
        Chicken,
        Staeak,
        Pepperoni,
        Egg,
        Milk,
        Cheese,
        Burgerpatty,
        count
        //Spinach,
        //Cabbage,
        //Pasta,
        //Beef,
        //Pork,
        //Bacon,
        //FishSalmon,
        //FishTuna,
        //Shrimp,
        //Meatballs,
        //Butter,
        //Apple,
        //Banana,
        //Strawberry,
        //Lemon,
        //Orange,
        //Pineapple,
        //Mango,
        //Cherry,
        //Blueberry,
        //Grapes,
        //Watermelon,
        //Peach,
    }
    [System.Serializable]
    public enum ProcessStatus
    {
        None = 0,
        Chopped = 1,
        FineChopped = 2,
        Boiled = 3,
        HalfBoiled = 4,
        Fryed = 5,
        UnderCooked = 6,
        Cooked = 7,
        OverCooked = 8,
        Burned = 9,
    }

    public enum Dishes
    {
        ChickenCurry,
        VegetableCurry,
        Burger,
        TomatoSoup,
        trash,
        count,
    }


    [System.Serializable]
    public struct ProcedureStep
    {
        public IngredientType Ingredient;
        public ProcessStatus processed;
        public string Descriptions;// Testing purpose only

    }
    [System.Serializable]
    public class Recipes
    {
        public List<ProcedureStep> items;
        public Dishes OutputDish;
        public int CookingTime;
        public float price;
        public bool IsthisDish(List<ProcedureStep> _items)
        {
            if(items.Count!=_items.Count) return false;
            var templist = GetthisDishIngreident();
            foreach (var item in _items)
            {
                if (!templist.Contains(item.Ingredient))
                {
                    return false;
                }
                else
                {
                    templist.Remove(item.Ingredient);
                }
            }
            return true;
        }
        public bool CanBeThisDish(List<ProcedureStep> _items)
        {
            if (items.Count < _items.Count) return false;
            var templist = GetthisDishIngreident();// for duplicate check
            foreach (var item in _items)
            {
                if (!templist.Contains(item.Ingredient))
                {
                    return false;
                }
                else
                {
                    templist.Remove(item.Ingredient);
                }
            }
            return true;
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
        public float CalculatePriceBonus(List<ProcedureStep> _items)
        {
            float multi = 0;
            foreach (var item in _items)
            {
                multi += isIngreidentProcessedcorrectly(item);
            }
            return multi;
        }
        //Quality Check here
        float isIngreidentProcessedcorrectly(ProcedureStep _step)
        {
            foreach (var item in items)
            {
                if (item.Ingredient.Equals(_step.Ingredient))
                {
                    //Finechopping and chopping are same thing but a different Result 
                    if (item.processed == ProcessStatus.Chopped)
                    {
                        if (_step.processed == ProcessStatus.Chopped)// normal bonus
                            return 1f;
                        if (_step.processed == ProcessStatus.FineChopped)// perfect bonus
                            return 1.5f;
                    }
                    if (item.processed == ProcessStatus.Boiled)
                    {
                        if (_step.processed == ProcessStatus.HalfBoiled)// normal bonus
                            return 1f;
                        if (_step.processed == ProcessStatus.Boiled)// perfect bonus
                            return 1.5f;
                    }
                    if (item.processed == ProcessStatus.Cooked)
                    {
                        if (_step.processed == ProcessStatus.OverCooked)// normal bonus
                            return 1f;
                        if (_step.processed == ProcessStatus.Cooked)// perfect bonus
                            return 1.5f;
                        if (_step.processed == ProcessStatus.UnderCooked)// worst bonus
                            return 0.75f;
                    }
                    if (item.processed == _step.processed) return 1.5f;

                }
            }
            return 0;
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

        public string PrefabString;
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
