using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControle : MonoBehaviour
{
    public GameObject Target;
    public GameObject Rig;
    public Transform IdleTransfromPosition,MovementTransformPosition;
    public Transform SafeAreaMin, SafeAreaMax;
    [SerializeField] Camera _cam;
    private void Update()
    {
        MoveCam();
    }
    //void CheckSafeArea()
    //{
    //    Debug.DrawRay(_cam.transform.position, _cam.ViewportPointToRay(Vector3.zero).direction, Color.red,1);
    //    Debug.DrawRay(_cam.transform.position, _cam.ViewportPointToRay(new Vector3(1,1,0)).direction, Color.yellow,1);
    //    Debug.DrawRay(_cam.transform.position, _cam.ViewportPointToRay(new Vector3(1,0,0)).direction, Color.white,1);
    //    Debug.DrawRay(_cam.transform.position, _cam.ViewportPointToRay(new Vector3(0,1,0)).direction, Color.cyan,1);
    //    RaycastHit hit1,hit2;
    //    var Ray1 = _cam.ViewportPointToRay(Vector3.zero);
    //    var Ray2 = _cam.ViewportPointToRay(new Vector3(1, 0, 0));
    //    //var Ray3 = Camera.main.ViewportPointToRay(new Vector3(0, 1, 0));
    //    if (Physics.Raycast(Ray1, out hit1, 10000f, mask))
    //    {
    //        backwall = hit1.transform.tag == "Backwall";
    //        leftwall = hit1.transform.tag == "Leftwall";
    //        rightwall = false;
    //        Debug.Log($"left:{leftwall},right:{rightwall},Backwall{backwall} ");
    //        //return false;
    //    }
    //    if (Physics.Raycast(Ray2, out hit2, 10000f, mask))
    //    {
    //        backwall = hit2.transform.tag == "Backwall";
    //        rightwall = hit2.transform.tag == "Rightwall";
    //        leftwall = false;
    //        Debug.Log($"left:{leftwall},right:{rightwall},Backwall{backwall} ");
    //        //return false;
    //    }
    //    //return true;
    //}
    void MoveCam()
    {        
        var pos = new Vector3(0, 0, 0);       
        Rig.transform.position  = new Vector3(SafeAreaMin.position.x <= Mathf.Abs(Target.transform.position.x)&& SafeAreaMax.position.x >= Mathf.Abs(Target.transform.position.x) ? Target.transform.position.x : Rig.transform.position.x, Target.transform.position.y,
           SafeAreaMin.position.z <= Mathf.Abs(Target.transform.position.z) && SafeAreaMax.position.z >= Mathf.Abs(Target.transform.position.z) ? Target.transform.position.z : Rig.transform.position.z);
        _cam.transform.position = MovementTransformPosition.position;
        
    }
}
