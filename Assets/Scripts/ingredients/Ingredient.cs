using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour, IHandHeld
{
    public ProcedureStep ingrendient;
    private typeofhandheld type;
    public Mesh mesh;
    public MeshRenderer Renderer;
    public MeshFilter Filter;

    public void AddEvent()
    {
        InputManager.Instance.Interactionbtn.ResetButton("Interact");
        return;
    }

    public void Setup(Mesh _mesh,IngredientType _type, typeofhandheld handtype,Material mat, processIngredient process=processIngredient.None)
    {
        ingrendient.Ingredient = _type;
        ingrendient.processed = process;
        type = handtype;
        this.mesh = _mesh;
        Filter.sharedMesh = this.mesh;
        Renderer.sharedMaterial = mat;
    }
   

    public object CustomEventReturner()
    {
        throw new System.NotImplementedException();
    }

    public void DestroyMe()
    {
        Destroy(this.gameObject);
        //Object pooling here
    }

    public typeofhandheld IGetType() => type;

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
