using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Dish : MonoBehaviour
{
    [System.Serializable]
    public class DishVisual
    {
        public Dishes dish;
        public GameObject go;
        
        public void Activate()
        {
            go.SetActive(true);
        }
        public void deActivate()
        {
            go.SetActive(false);
        }
    }
    public List<DishVisual> Visuals;

    public void DeactivateAll()
    {
        foreach (DishVisual v in Visuals)
        {
            v.deActivate();
        }
    }

    public void SetaDishActive(Dishes dish)
    {
        foreach (DishVisual v in Visuals)
        {
            if (v.dish == dish)
            {
                v.Activate();
            }
        }
    }
}

