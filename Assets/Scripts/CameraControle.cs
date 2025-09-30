using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControle : MonoBehaviour
{
    public float CameraSmoothValue = 0.3f;
    public GameObject Target;
    public GameObject Rig;
    public Transform IdleTransfromPosition,MovementTransformPosition;
    public Transform SafeAreaMin, SafeAreaMax;
    [SerializeField] Camera _cam;
    private void Update()
    {
        MoveCam();
    }
    void MoveCam()
    {        
        var pos = new Vector3(0, 0, 0);     
        var newposition= new Vector3(SafeAreaMin.position.x <= Mathf.Abs(Target.transform.position.x) && SafeAreaMax.position.x >= Mathf.Abs(Target.transform.position.x) ? Target.transform.position.x : Rig.transform.position.x, Target.transform.position.y,
           SafeAreaMin.position.z <= Mathf.Abs(Target.transform.position.z) && SafeAreaMax.position.z >= Mathf.Abs(Target.transform.position.z) ? Target.transform.position.z : Rig.transform.position.z);
        Rig.transform.position = Vector3.Lerp(Rig.transform.position, newposition, Time.deltaTime * CameraSmoothValue);
        _cam.transform.position = MovementTransformPosition.position;
    }
    public void SetTarget(GameObject _target)
    {
        Target = _target;
    }
}
public enum CameraTargetMode
{
    PlayerCam,FreeCam,BuildingMode
}
