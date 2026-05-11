using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using System;
[CreateAssetMenu(fileName = "Ingredient", menuName = "IngredientSprite")]

public class SO_IngredientData : ScriptableObject
{
    public IngredientType type;
    public typeofhandheld handheldtype;
    public ProcessStatus process;
    public Sprite icon;
    public Mesh mesh;
    public Material material;
}
[Serializable]
public struct ProcessingDataForMeshes
{
    public ProcessStatus process;
    public Sprite icon;
    public Mesh mesh;
    public Material material;
}