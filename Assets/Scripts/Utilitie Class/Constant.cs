using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        IceCreamDispenser,
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
        none,
        count,
    }

    [System.Serializable]
    public struct ObjectLevelDetails
    {
        public int Level;
        public string Name;
        public string Icon;
        public List<string> Description;
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
        public int[] arr;
        public bool IsthisDish(List<ProcedureStep> _items)
        {
            if (items.Count != _items.Count) return false;
            var templist = GetthisDishIngreident();
            Debug.Log(CustomLogs.CC_TagLog("Frying Pan", $"checking internal{_items.Count},{templist.Count}"));
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
            return multi / _items.Count;
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
                    if (item.processed == ProcessStatus.Fryed)
                    {
                        if (_step.processed == ProcessStatus.OverCooked)// normal bonus
                            return 1f;
                        if (_step.processed == ProcessStatus.Fryed)// perfect bonus
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
        public int GetTaxsurgeFee() => (data.IncomeTax / 100) * data.TotalIncome;
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

    public class Npc_Review
    {
        public string[] PositiveReview = {
"The {0} was absolutely delicious!",
"I loved every bite of that {0}!",
"That {0} was exactly what I was craving.",
"The {0} was fantastic. I’d order it again!",
"Whoever made this {0} deserves a compliment.",
"The {0} was even better than I expected.",
"That was one of the best {0} I’ve had in a long time.",
"The {0} was perfect from start to finish.",
"I’m definitely coming back for another {0}.",
"That {0} really made my day.",
"The {0} was packed with flavour!",
"I couldn’t stop eating that {0}!",
"The {0} was absolutely worth the price.",
"Now that’s how you make a good {0}!",
"I didn’t expect the {0} to be this good.",
"The {0} was incredible. Five stars!",
"I’m already thinking about my next {0}.",
"That {0} was cooked perfectly.",
"The {0} tasted amazing!",
"I have no complaints. That {0} was perfect.",
"The {0} was exactly what I needed today.",
"I’d recommend this {0} to anyone.",
"That was a seriously tasty {0}.",
"The {0} exceeded my expectations.",
"I came hungry and left very happy!",
"That {0} was worth every bite.",
"I could honestly eat another one right now.",
"The {0} was delicious and satisfying.",
"I’m giving that {0} a solid five stars!",
"Absolutely wonderful {0}. I’ll be back!",
        };
        public string[] NegativeReview = {
"I really didn’t enjoy that {0}.",
"The {0} was disappointing.",
"I expected much better from the {0}.",
"That was probably the worst {0} I’ve had.",
"The {0} just didn’t taste right.",
"I couldn’t finish the {0}.",
"Something was definitely wrong with that {0}.",
"The {0} was a huge disappointment.",
"I wouldn’t recommend the {0}.",
"The {0} was completely underwhelming.",
"I regret ordering the {0}.",
"The {0} really wasn’t good.",
"I expected more from this place.",
"The {0} was lacking flavour.",
"I’ve had much better {0} elsewhere.",
"The {0} was not worth the money.",
"I don’t think I’ll be ordering that again.",
"The {0} was a letdown.",
"I struggled to finish that {0}.",
"The {0} tasted nothing like I expected.",
"I’m not sure what happened with that {0}.",
"The {0} definitely needs some improvement.",
"I wasn’t impressed with the {0}.",
"That {0} left a lot to be desired.",
"I wish I had ordered something else.",
"The {0} was far from satisfying.",
"I really wanted to like the {0}.",
"That was not a good {0}.",
"The {0} needs some serious work.",
"I won’t be rushing back for another {0}.",
        };
        public string[] NeutralReview = {
"The {0} was pretty good.",
"The {0} was okay. Nothing special.",
"I’ve had better {0}, but this was fine.",
"The {0} was decent.",
"Not bad. I’d probably order it again.",
"The {0} was alright.",
"It wasn’t amazing, but it did the job.",
"The {0} was okay for the price.",
"I didn’t love it, but I didn’t hate it either.",
"The {0} was fairly decent.",
"Nothing to complain about with this {0}.",
"The {0} was fine. Just fine.",
"It was an acceptable {0}.",
"The {0} tasted alright.",
"I expected a little more from the {0}.",
"The {0} was decent, but forgettable.",
"Not the best {0}, but certainly not the worst.",
"The {0} filled me up, at least.",
"It was a perfectly average {0}.",
"The {0} was okay, I suppose.",
"I might order the {0} again sometime.",
"The {0} was nothing extraordinary.",
"It tasted fine to me.",
"The {0} could have been better.",
"I’m not sure how I feel about that {0}.",
"The {0} was acceptable.",
"It was an okay meal.",
"The {0} wasn’t bad at all.",
"I guess the {0} was pretty decent.",
"Nothing special, but I finished it.",
        };
        public string[] FunnyReview = {
"That {0} was so good I forgot where I was.",
"I came for food and left with a new obsession with {0}.",
"My stomach has officially requested another {0}.",
"I’d fight a raccoon for another {0}.",
"That {0} just changed my life.",
"I blinked and the {0} was gone.",
"Who needs friends when you have {0}?",
"I think I just discovered my new favourite food.",
"That {0} deserves its own holiday.",
"I would like to personally thank whoever made that {0}.",
"My diet has been cancelled because of this {0}.",
"I was planning to share the {0}. Then I remembered I’m hungry.",
"That {0} disappeared faster than my paycheck.",
"I’m pretty sure I just tasted happiness.",
"Please don’t tell my doctor about this {0}.",
"I need another {0}. This is an emergency.",
"That {0} was suspiciously delicious.",
"I may have a serious {0} addiction now.",
"I ordered one {0}. Big mistake. Now I want ten.",
"My stomach is already asking for an encore.",
"That {0} was better than my last five dates.",
"I would sell my collection of spoons for another {0}.",
"Whoever invented {0} deserves a statue.",
"I don’t know what was in that {0}, but I need the recipe.",
"I came here hungry. I leave here happier and slightly poorer.",
"That {0} hit my taste buds like a truck.",
"I think my soul just left my body after that {0}.",
"Forget the menu. Just give me another {0}.",
"That {0} was so good I almost started crying.",
"I have absolutely no regrets about that {0}.",
        };
        public string GetReview()
        {
            return " ";
        }
    }
}
