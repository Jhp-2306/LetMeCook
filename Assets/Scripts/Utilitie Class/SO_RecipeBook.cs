using System.Collections.Generic;
using UnityEngine;
namespace Constants
{
    [CreateAssetMenu(fileName = "Recipe_Book", menuName = "ScriptableObject/RecipeBook")]
    public class SO_RecipeBook:ScriptableObject
    {
        public List<Recipes> items;

    }

    
}
