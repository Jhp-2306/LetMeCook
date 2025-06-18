using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour, IHandHeld
{
    [Serializable]
    public struct Str_Ingredient_Data
    {
    public IngredientType ingredientType;
    public typeofhandheld type;
    public processIngredient processes;
    public Mesh _mesh;

    }
    public Str_Ingredient_Data ingrendient;
    public MeshRenderer Renderer;
    public MeshFilter Filter;


    public void Setup(Str_Ingredient_Data copy)
    {
        ingrendient.ingredientType = copy.ingredientType;
        ingrendient.type = copy.type;
        ingrendient.processes = copy.processes;
        ingrendient._mesh=copy._mesh;
    }

    public void AddEvent()
    {
        return;
    }

    public void Setup(Mesh mesh,IngredientType _type, typeofhandheld handtype)
    {
        ingrendient.ingredientType = _type;
        ingrendient.type = handtype;
        ingrendient._mesh = mesh;
        Filter.sharedMesh = ingrendient._mesh;
        
    }
   

    public object CustomEventReturner()
    {
        throw new System.NotImplementedException();
    }

    public void DestroyMe()
    {
        Destroy(this.gameObject);
    }

    public typeofhandheld IGetType() => ingrendient.type;
    
}
