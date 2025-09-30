using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class AssetLoader : Singletonref<AssetLoader>
{
    public List<Sprite> sprites;

    public List<SO_IngredientData> ingredients;

    public Sprite iconMissing;

    public GameObject itemPrefab;
    public Sprite GetSpriteFromList(int index) => sprites[index];
    public SO_IngredientData GetIngredientSO(IngredientType type, processIngredient process=processIngredient.None)
    {
        foreach (var ing in ingredients) {
            if(process==ing.process)
            if (ing.type == type) return ing;
        }
        return null;
    }
    public Sprite GetIngredientIcon(IngredientType type, processIngredient process = processIngredient.None)
    {
        foreach (var ing in ingredients)
        {
            if (process == ing.process)
                if (ing.type == type) return ing.icon;
        }
        return iconMissing;
    }
}
