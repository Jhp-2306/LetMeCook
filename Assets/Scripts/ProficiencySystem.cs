using Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements.Experimental;
using Util;

public class ProficiencySystem : Singletonref<ProficiencySystem>
{
    //[System.Serializable]
    public class ProficiencyStats
    {
        public Dishes Dish;
        public int XP;
        public string recipe;
        public Dictionary<IngredientType, int> Ingredients;
        public bool isUnlocked()
        {
            foreach (var item in Ingredients.Keys)
            {
                if (!GameDataDNDL.Instance.unlockedIngredient.Contains(item))
                    return false;
            }
            return true;
        }
        public ProficiencyStats(Dishes dish, Dictionary<IngredientType, int> unlockconditions, string _recipe = null)
        {
            this.Dish = dish;
            Ingredients = new Dictionary<IngredientType, int>(unlockconditions);
            recipe = _recipe;

        }
        
        public int GetLevel()
        {
            var temp=ProficiencySystem.Instance.XPRequried.Keys.ToList();
            foreach (int xp in temp)
            {
                if (XP < xp) return temp.IndexOf(xp)+1;               
            }
           return 0;
        }
        public float GetMultipler()
        {
            var temp = ProficiencySystem.Instance.XPRequried.Keys.ToList();
            foreach (int xp in temp)
            {
                if (XP < xp) return ProficiencySystem.Instance.XPRequried[xp];
            }
            return 0;
        }
        public float GetXpMaxCap()
        {
            var temp = ProficiencySystem.Instance.XPRequried.Keys.ToList();
            foreach (int xp in temp)
            {
                if (XP < xp) return xp;
            }
            return 0;
        }
        public float GetCurrentXp()
        {
            var temp = ProficiencySystem.Instance.XPRequried.Keys.ToList();
            int idx=0;
            foreach (int xp in temp)
            {
                if (XP < xp)
                {
                    if(idx != 0)
                    {
                        return XP-temp[idx-1];
                    }
                }
                idx++;
            }
            return 0;
        }
        public void AddXP(int xp) {
            XP += xp;
        }
    }
    private Dictionary<Dishes, ProficiencyStats> dictionary;
    private List<ProficiencyStats> m_UnlockedDishes;
    private Dictionary<int, float> XPRequried;

    public static Action OnInit = delegate { };

    public List<Pc_ProficiencySystemWindow_Slider> slider;
    public void init()
    {
        if (XPRequried == null)
        {
            XPRequried = new Dictionary<int, float>()
            {
                {100  ,0.05f},
                {200  ,0.1f},
                {400  ,0.1f},
                {800  ,0.2f},
                {1600 ,0.3f}
            };
        }
        if (dictionary == null)
        {
            //Todo: Set he original data for the dishes
            dictionary = new Dictionary<Dishes, ProficiencyStats>();

            //chickenCurry
            var temp = new ProficiencyStats(Dishes.ChickenCurry,
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 1 }, { IngredientType.Onion, 1 }, { IngredientType.Chicken, 1 } }
                , "Chop a Tomato, Onion and a Chicken.\nAdd them to stove's Pot.\nCook them together in the pot until the Perfect (Green) indicator.");
            /*new ProficiencyStats(Dishes.ChickenCurry, new Dictionary<IngredientType, int> { { IngredientType.Tomato, 1 }, { IngredientType.Onion, 1 }, { IngredientType.Chicken, 1 } }));*/
            dictionary.Add(Dishes.ChickenCurry, temp);
            
            //Veg.curry
            temp = new ProficiencyStats(Dishes.VegetableCurry,
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 1 }, { IngredientType.Onion, 1 }, { IngredientType.Carrot, 1 } }
                , "Chop a Tomato, Onion and a Carrot.\nAdd them to stove's Pot.\nCook them together in the pot until the Perfect (Green) indicator.");
            dictionary.Add(Dishes.VegetableCurry, temp);
            
            //burger
            temp = new ProficiencyStats(Dishes.Burger, 
                new Dictionary<IngredientType, int> { { IngredientType.BurgerBun, 1 }, { IngredientType.Burgerpatty, 1 }, { IngredientType.Lettuce, 1 }, { IngredientType.Cheese, 1 } }
                , "Slice the burger bun.\nFry the burger patty until the Perfect (Green) indicator.\nChop the lettuce and cheese.\nAssemble all ingredients onto the bun");
            dictionary.Add(Dishes.Burger, temp);
            
            //Tomato soup
            temp = new ProficiencyStats(Dishes.TomatoSoup, 
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 2 } }
                , "Chop both tomatoes.\nPlace them in the pot.\nCook until the Perfect (Green) indicator appears.");
            dictionary.Add(Dishes.TomatoSoup, temp);
            
            //Mashedpotato
            temp = new ProficiencyStats(Dishes.MashedPotato, 
                new Dictionary<IngredientType, int> { { IngredientType.Potato, 2 } }
                , "Chop both Potatoes.\nPlace them in the pot.\nCook until the Perfect (Green) indicator appears.");
            dictionary.Add(Dishes.MashedPotato, temp);
            
            //Mushroom Curry
            temp = new ProficiencyStats(Dishes.MushroomCurry, 
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 1 }, { IngredientType.Onion, 1 }, { IngredientType.Mushroom, 1 } }
                , "Chop a Tomato, Onion and a Mushroom.\nAdd them to stove's Pot.\nCook them together in the pot until the Perfect (Green) indicator.");
            dictionary.Add(Dishes.MushroomCurry, temp);
            
            //Veg soup
            temp = new ProficiencyStats(Dishes.VegetableSoup, 
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 2 }, { IngredientType.Onion, 1 } }
                , "Chop two Tomato and a Onion.\nAdd them to stove's Pot.\nCook them together in the pot until the Perfect (Green) indicator.");
            dictionary.Add(Dishes.VegetableSoup, temp);
            
            //chicken soup
            temp = new ProficiencyStats(Dishes.ChickenSoup, 
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 2 }, { IngredientType.Chicken, 1 } }
                , "Chop two Tomato and a Chicken.\nAdd them to stove's Pot.\nCook them together in the pot until the Perfect (Green) indicator.");
            dictionary.Add(Dishes.ChickenSoup, temp);
            
            //Fired Chicken Rice
            temp = new ProficiencyStats(Dishes.FiredChickenRice, 
                new Dictionary<IngredientType, int> { { IngredientType.Rice, 1 }, { IngredientType.Onion, 1 }, { IngredientType.Chicken, 1 } }
                , "Chop a Onion and a Chicken.\nAdd them with Rice to stove's Pot.\nCook them together in the pot until the Perfect (Green) indicator.");
            dictionary.Add(Dishes.FiredChickenRice, temp);
            
            //Fired Veg Rice
            temp = new ProficiencyStats(Dishes.FiredVegetableRice, 
                new Dictionary<IngredientType, int> { { IngredientType.Rice, 1 }, { IngredientType.Onion, 1 }, { IngredientType.Carrot, 1 } }
                , " Chop a Onion and a Carrot.\nAdd them with Rice to stove's Pot.\nCook them together in the pot until the Perfect (Green) indicator.");
            dictionary.Add(Dishes.FiredVegetableRice, temp);
           

            //Fired Chicken
            temp = new ProficiencyStats(Dishes.FiredChicken, 
                new Dictionary<IngredientType, int> { { IngredientType.Chicken, 1 } }
                , "Fry the Chicken until the Perfect (Green) indicator. ");
            dictionary.Add(Dishes.FiredChicken, temp);
            

            //Salad
            temp = new ProficiencyStats(Dishes.Salad, 
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 1 }, { IngredientType.Lettuce, 1 }, { IngredientType.Carrot, 1 } }
                , "Chop a Tomato, Lettuce and a Carrot.\nArrange them on a plate.");
            dictionary.Add(Dishes.Salad, temp);
            
            // Rice and chicken curry
            temp = new ProficiencyStats(Dishes.RiceAndChickenCurry, 
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 1 }, { IngredientType.Onion, 1 }, { IngredientType.Chicken, 1 }, { IngredientType.Rice, 1 } }
                , "Prepare the Chicken Curry.\nPlace rice onto a plate.\nAdd the curry and serve.");
            dictionary.Add(Dishes.RiceAndChickenCurry, temp);
            

            //Rice and Veg Curry
            temp = new ProficiencyStats(Dishes.RiceAndVegetableCurry, 
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 1 }, { IngredientType.Onion, 1 }, { IngredientType.Carrot, 1 }, { IngredientType.Rice, 1 } }
                , "Prepare the Vegetable Curry.\nPlace rice onto a plate.\nAdd the curry and serve. ");
            dictionary.Add(Dishes.RiceAndVegetableCurry, temp);
            
            //Rice and Mushroom
            temp = new ProficiencyStats(Dishes.RiceAndMushroomCurry, 
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 1 }, { IngredientType.Onion, 1 }, { IngredientType.Mushroom, 1 }, { IngredientType.Rice, 1 } }
                , "Prepare the Mushroom Curry.\nPlace rice onto a plate.\nAdd the curry and serve. ");
            dictionary.Add(Dishes.RiceAndMushroomCurry, temp);
            

            //MilkShake
            temp = new ProficiencyStats(Dishes.Milkshake, 
                new Dictionary<IngredientType, int> { { IngredientType.Milk, 1 } }, "Pour milk into the blender.\nBlend until smooth.\n ");
            dictionary.Add(Dishes.Milkshake, temp);
            
            //Chocolate MilkShake
            temp = new ProficiencyStats(Dishes.ChocolateMilkshake, 
                new Dictionary<IngredientType, int> { { IngredientType.Chocolate, 1 }, { IngredientType.Milk, 1 } }
                , "Add milk and one scoop of chocolate ice cream to the blender.\nBlend until smooth.");
            dictionary.Add(Dishes.ChocolateMilkshake, temp);
            
            //Vanilla Milkshake
            temp = new ProficiencyStats(Dishes.VanillaMilkshake, 
                new Dictionary<IngredientType, int> { { IngredientType.Vanillia, 1 }, { IngredientType.Milk, 1 } }
                , "Add milk and one scoop of vanilla ice cream to the blender.\nBlend until smooth.");
            dictionary.Add(Dishes.VanillaMilkshake, temp);
           

            //Strawberry Milkshake
            temp = new ProficiencyStats(Dishes.StrawberryMilkshake, 
                new Dictionary<IngredientType, int> { { IngredientType.Strawberry, 1 }, { IngredientType.Milk, 1 } }
                , "Add milk and one scoop of strawberry ice cream to the blender.\nBlend until smooth. ");
            dictionary.Add(Dishes.StrawberryMilkshake, temp);
            
            //Sandwich
            temp = new ProficiencyStats(Dishes.Sandwich, 
                new Dictionary<IngredientType, int> { { IngredientType.Tomato, 1 }, { IngredientType.Lettuce, 1 }, { IngredientType.Bread, 1 }, { IngredientType.Cheese, 1 } }
                , "Slice the bread.\nChop the tomato, lettuce, and cheese.\nAssemble everything on the bread. ");
            dictionary.Add(Dishes.Sandwich, temp);
            
            //Egg Omelette
            temp = new ProficiencyStats(Dishes.EggOmelette, 
                new Dictionary<IngredientType, int> { { IngredientType.Egg, 1 } }
                , " Fry the egg until the Perfect (Green) indicator.\r\n");
            dictionary.Add(Dishes.EggOmelette, temp);
            //dictionary.Add(Dishes.EggOmblet,           temp); 
            //temp =/*new ProficiencyStats(Dishes.EggOmblet, new Dictionary<IngredientType, int> { IngredientType.Tomato, IngredientType.Onion, IngredientType.Chicken }));*/
            
            //Toasted Bread with Cheese
            temp = new ProficiencyStats(Dishes.ToastedBreadWithCheese, 
                new Dictionary<IngredientType, int> { { IngredientType.Bread, 1 }, { IngredientType.Cheese, 1 } }
                , " Slice the bread.\nToast it until the Perfect (Green) indicator.\nChop the cheese.\nPlate together.");
            dictionary.Add(Dishes.ToastedBreadWithCheese, temp);
           
            //Steak mid
            temp = new ProficiencyStats(Dishes.SteakMid, 
                new Dictionary<IngredientType, int> { { IngredientType.Steak, 1 } }
                , "Fry the steak until the Medium (Green) indicator.");
            dictionary.Add(Dishes.SteakMid, temp);
            

        //steak well
            temp = new ProficiencyStats(Dishes.SteakWell, 
                new Dictionary<IngredientType, int> { { IngredientType.Steak, 1 } }
                , "Fry the steak until the Well Done (Red) indicator.");
            dictionary.Add(Dishes.SteakWell, temp);
            
            //steak Rara
            temp = new ProficiencyStats(Dishes.SteakRare, 
                new Dictionary<IngredientType, int> { { IngredientType.Steak, 1 } }
                , "Fry the steak until the Rare (Blue) indicator.");
            dictionary.Add(Dishes.SteakRare, temp);
            
            //Bread Omelette
            temp = new ProficiencyStats(Dishes.BreadOmelette, 
                new Dictionary<IngredientType, int> { { IngredientType.Bread, 1 }, { IngredientType.Egg, 1 } }
                , "Fry the egg until the Perfect indicator.\nSlice the bread.\nPlate together.");
            dictionary.Add(Dishes.BreadOmelette, temp);

            //Bread Omelette With Cheese
            temp = new ProficiencyStats(Dishes.BreadOmeletteWithCheese, 
                new Dictionary<IngredientType, int> { { IngredientType.Bread, 1 }, { IngredientType.Egg, 1 }, { IngredientType.Cheese, 1 } }
                , "Fry the egg until the Perfect indicator.\nSlice the bread.\nChop the cheese.\nPlate everything together. ");
            dictionary.Add(Dishes.BreadOmeletteWithCheese, temp);
            
            //Chocolate Ice Cream
            temp = new ProficiencyStats(Dishes.ChocolateIceCream, 
                new Dictionary<IngredientType, int> { { IngredientType.Chocolate, 1 }, }
                , "Scoop one serving of chocolate ice cream.\nPlace it on a plate. ");
            dictionary.Add(Dishes.ChocolateIceCream, temp);
            

        // Vanilla Ice Cream
            temp = new ProficiencyStats(Dishes.VanillaIceCream, 
                new Dictionary<IngredientType, int> { { IngredientType.Vanillia, 1 }, }
                , "Scoop one serving of vanilla ice cream.\nPlace it on a plate. ");
            dictionary.Add(Dishes.VanillaIceCream, temp);
            
            //Strawberry Ice Cream
            temp = new ProficiencyStats(Dishes.StrawberryIceCream, 
                new Dictionary<IngredientType, int> { { IngredientType.Strawberry, 1 } }
                , "Scoop one serving of strawberry ice cream.\nPlace it on a plate. ");
            dictionary.Add(Dishes.StrawberryIceCream, temp);
        }
        
        //TODO: Get the Save From the Doc       
        if (m_UnlockedDishes == null) //if Fail to retrive save data then Run this
        {
            m_UnlockedDishes = new List<ProficiencyStats>();
            foreach (var dish in dictionary)
            {
                if (dish.Value.isUnlocked()) m_UnlockedDishes.Add(dish.Value);
            }

        }
        foreach(var temp in slider)
        {
            temp.EventSub();

        }
        OnInit();
    }
    public List<ProficiencyStats> getUnlockedDishStats()=>m_UnlockedDishes;
    public List<Dishes> getUnlockDish()
    {
        var temp = new List<Dishes>();
        foreach (var dish in m_UnlockedDishes)
        {
            temp.Add(dish.Dish);
        }
        return temp;
    }
    public ProficiencyStats GetProficiencyStats(Dishes dishes) => dictionary[dishes];
}





