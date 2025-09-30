using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
     //public List<MeshRenderer> renderers;
    public Material Green, Red;
    public List<GameObject> go;
    private void Start()
    {
        ResetGO();
    }
    public void set(bool canPlace,List<Vector3> positiondisplacement)
    {
        //var selectedmaterials = canPlace ? Green : Red;

        //for (int i = 0; i <= positiondisplacement.Count; i++)
        //{

        //    go[i].transform.localPosition =new Vector3(positiondisplacement[i].x, 0f,positiondisplacement[i].z);
        //    go[i].GetComponent<MeshRenderer>().sharedMaterial= selectedmaterials;
        //    go[i].SetActive(true);
        //}
    }
    public void ResetGO()
    {
        //foreach (gameobject gobj in go)
        //{
        //    gobj.setactive(false);
        //    gobj.transform.localposition = vector3.zero;
        //}
        //go[0].transform.localposition = vector3.zero;
        //go[0].setactive(true);
    }
}
