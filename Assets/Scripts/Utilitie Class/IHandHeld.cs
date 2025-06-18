using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandHeld 
{
    public typeofhandheld IGetType();

    public void AddEvent();

    public object CustomEventReturner();
    
    public void DestroyMe();
}
