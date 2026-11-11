using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Ingredient : MonoBehaviour, IHandHeld, IObjectPoolingSettings
{
    public ProcedureStep ingrendient;
    private typeofhandheld type;
    public Mesh mesh;
    public MeshRenderer Renderer;
    public MeshFilter Filter;
    public List<IngredientType> IngredientsRequires2Scale;
    public void AddEvent()
    {
        InputManager.Instance.Interactionbtn.ResetButton("Interact");
        return;
    }

    public void Setup(Mesh _mesh, IngredientType _type, typeofhandheld handtype, Material mat, ProcessStatus process = ProcessStatus.None)
    {
        ingrendient.Ingredient = _type;
        ingrendient.processed = process;
        if (IngredientsRequires2Scale.Contains(_type))
        {
            this.transform.localScale = Vector3.one * 2;
        }
        else
        {
            this.transform.localScale = Vector3.one * 0.5f;
        }
        type = handtype;
        this.mesh = _mesh;
        Filter.sharedMesh = this.mesh;
        Renderer.sharedMaterial = mat;
    }


    public void Setup(SO_IngredientData data)
    {
        ingrendient.Ingredient = data.type;
        ingrendient.processed = data.process;
        type = data.handheldtype;
        this.mesh = data.mesh;
        Filter.sharedMesh = this.mesh;
        Renderer.sharedMaterial = data.material;
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

    public void OPSetting_Reset()
    {
        //throw new NotImplementedException();
        this.gameObject.SetActive(false);
    }
}
