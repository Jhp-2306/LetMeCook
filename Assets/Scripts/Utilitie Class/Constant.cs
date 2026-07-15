using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;


namespace Constants
{
    public static class Constant
    {
        public static readonly string DateTimeFormate = "MM/dd/yyyy HH:mm";

        public static string EO = "Equipment Owned ...{0}";
        public static string CpE = "Usage Cost per Equipment ...{0}";
        public static string CR = "Customer Recived ...{0}";
        public static string IE = "Income Earned ...{0}";
        public static string ITp = "Income Tax Percentage ...{0}";
        public static string D = "Difficulty ...{0}";
        public static string DB = "Difficulty Bonus ...{0}";
        public static string RGT = "Random govt. Tax ...{0}";
        public static string EF = "Equipment Fee ...{0}";
        public static string TF = "Tax Fee...{0}";
        public static string DD = "Discount...{0}";
        public static string FA = "Final Amount ...{0}";


    }

    public static class StringExtensions
    {
        public static string CamelCaseToWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return Regex.Replace(
                text,
                @"(?<=[a-z])([A-Z])|(?<=[A-Z])(?=[A-Z][a-z])",
                " $1"
            ).Trim();
        }
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
        Steak,
        Pepperoni,
        Egg,
        Milk,
        Cheese,
        Burgerpatty,
        Chocolate,
        Vanillia,
        Strawberry,
        IceCream,
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
        MashedPotato,
        MushroomCurry,
        VegetableSoup,
        ChickenSoup,
        FiredChickenRice,
        FiredVegetableRice,
        FiredChicken,
        Salad,
        RiceAndChickenCurry,
        RiceAndVegetableCurry,
        RiceAndMushroomCurry,
        Milkshake,
        ChocolateMilkshake,
        VanillaMilkshake,
        StrawberryMilkshake,
        Sandwich,
        EggOmelette,
        ToastedBreadWithCheese,
        SteakMid,
        SteakWell,
        SteakRare,
        BreadOmelette,
        BreadOmeletteWithCheese,
        ChocolateIceCream,
        VanillaIceCream,
        StrawberryIceCream,
        trash,
        count,
    }

    [System.Serializable]
    public struct ObjectLevelDetails
    {
        public int Level;
        public string Name;
        public string Icon;
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
        public bool DishMix;
        public List<Dishes> dishes;
        public Dishes OutputDish;
        public int CookingTime;
        public float price;

        public bool IsthisDish(List<ProcedureStep> _items)
        {
            if (items.Count != _items.Count) return false;
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
    public struct PerkData
    {
        public int Rank;
        public string PerkName;
        public perkSystem_Value Type;
        public bool isBool;
        public int Value;
        public string Des;
    }

    [System.Serializable]
    public class SaveDataTemplate
    {
        public string id;
        public int level;
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

    public class Bill
    {
        private BillData data;

        public Bill(int equipmentOwne, int costPerEquipment, int totalCustomerAttended, int totalIncome, int incomeTax, int difficulty, int difficultyBonus, int randomGovtTax)
        {
            data.EquipmentOwne = equipmentOwne;
            data.CostPerEquipment = costPerEquipment;
            data.TotalCustomerAttended = totalCustomerAttended;
            data.TotalIncome = totalIncome;
            data.IncomeTax = incomeTax;
            data.Difficulty = difficulty;
            data.DifficultyBonus = difficultyBonus;
            data.RandomGovtTax = randomGovtTax;
        }
        public Bill(BillData data)
        {
            this.data = data;
        }

        public int GetEquipmentsurgeFee() => data.EquipmentOwne * data.CostPerEquipment;
        public int GetTaxsurgeFee() => (data.IncomeTax/100) * data.TotalIncome;
        public int GetBonusFeeDeduction() => data.Difficulty * data.DifficultyBonus;
        public int GetRandomGovtTax() => data.RandomGovtTax;

        // Gacha 
        public int GetRandomDiscountAfterAVideo()
        {
            data.RandomDiscount = 0;
            return 0;
        }
        public int GetTotal() => (GetEquipmentsurgeFee() + GetTaxsurgeFee() + GetRandomGovtTax()) - (GetBonusFeeDeduction() + GetRandomDiscountAfterAVideo());
        public BillData GetBillData() => data;
    }
    [System.Serializable]
    public struct BillData
    {
        public bool isBillPayed;
        public int EquipmentOwne;
        public int CostPerEquipment;
        public int TotalCustomerAttended;
        public int TotalIncome;
        public int IncomeTax;
        public int Difficulty;
        public int DifficultyBonus;
        public int RandomGovtTax;
        public int RandomDiscount;
    }

    public enum perkSystem_Value
    {
        EquipmentOwnePrice,
        IncomeTaxRate,
        Instant,
        BlackMarket,
        Auto,
        OrderWaitingPeroid,
        Govt_Tax,
        Count
    }
    

}
