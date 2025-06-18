using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
[CreateAssetMenu(fileName = "Ingredient", menuName = "IngredientSprite")]

public class SO_IngredientData : ScriptableObject
{
    public IngredientType type;
    public typeofhandheld handheldtype;
    public Sprite icon;
    public Mesh mesh;
}
