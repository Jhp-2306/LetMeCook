using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour, IHandHeld
{
    //public typeofhandheld type;
    public bool isPlateEmpty=true;
    public void AddEvent()
    {
        InputManager.Instance.Interactionbtn.ResetButton("Interact");
        return;
    }

    public object CustomEventReturner()
    {
        return null;
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public typeofhandheld IGetType()
    {
       return typeofhandheld.plate;
    }
}
