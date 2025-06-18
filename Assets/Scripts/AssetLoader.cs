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
    public SO_IngredientData GetIngredientSprite(IngredientType type)
    {
        foreach (var ing in ingredients) {
            if (ing.type == type) return ing;
        }
        return null;
    }
}
