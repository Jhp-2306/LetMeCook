using System.Collections;
using System;
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
    [SerializeField] private float Zoomval;
    bool zoomCam;
    float Animationtimer;
    public AnimationCurve ZoomCurve;
    float baseYval;
    public float checkval;
    bool callbackFlag;
    Action onReachCallback;
    [SerializeField] Vector3 offset;
    private void Awake()
    {
        baseYval = MovementTransformPosition.position.y;
    }
    private void Update()
    {
        MoveCam();
    }
    
    void MoveCam()
    {
       //CamZoom = ;
        if (Target.GetComponent<Player>()!=null)
        {
            var player=Target.GetComponent<PlayerMovement>();
            if (player.isPlayerMoving&& !zoomCam)
            {
                //_cam.fieldOfView=ZoomCurve.Evaluate(player.SpeeedAnimationTimer);
                //MovementTransformPosition.position=Vector3.Lerp(MovementTransformPosition.position,Vector3.up*16f,)
                zoomCam = true;
                Animationtimer = 0;
            }
            else if(!player.isPlayerMoving && zoomCam)
            {
                zoomCam= false;
                Animationtimer = 0;
            }
            if (zoomCam)
            {
                checkval = ZoomCurve.Evaluate(Animationtimer);
                MovementTransformPosition.position = Vector3.Lerp(MovementTransformPosition.position,
                    new Vector3(MovementTransformPosition.position.x, Mathf.Clamp(MovementTransformPosition.position.y + Zoomval, baseYval, baseYval+Zoomval), MovementTransformPosition.position.z), 
                    ZoomCurve.Evaluate(Animationtimer / ZoomCurve.keys[1].time));
            }
            else { 
                MovementTransformPosition.position = Vector3.Lerp(MovementTransformPosition.position, 
                    new Vector3(MovementTransformPosition.position.x,Mathf.Clamp(MovementTransformPosition.position.y - Zoomval, baseYval, baseYval + Zoomval),MovementTransformPosition.position.z),
                    ZoomCurve.Evaluate(Animationtimer / ZoomCurve.keys[1].time));            
            }
            Animationtimer += Time.deltaTime;
        }
        Vector3 targetPos = Rig.transform.position;

        bool insideX = Target.transform.position.x >= SafeAreaMin.position.x &&
                       Target.transform.position.x <= SafeAreaMax.position.x;

        bool insideZ = Target.transform.position.z >= SafeAreaMin.position.z &&
                       Target.transform.position.z <= SafeAreaMax.position.z;

        if (insideX)
            targetPos.x = Target.transform.position.x;

        if (insideZ)
            targetPos.z = Target.transform.position.z;

        Rig.transform.position = Vector3.Lerp(
            Rig.transform.position,
            targetPos+ offset,
            Time.deltaTime * CameraSmoothValue);

        _cam.transform.position = MovementTransformPosition.position;
        if (!callbackFlag && Vector3.Distance(Rig.transform.position,targetPos+offset)<0.25)
        {
           // Debug.Log(CustomLogs.CC_TagLog("CameraControle", "Reached the target point"));
            callbackFlag = true;
            onReachCallback?.Invoke();
            onReachCallback = null;
        }
    }
    public void SetTarget(GameObject _target, Action OnReach=null,Vector3 _offset=default)
    {
        Target = _target;
        callbackFlag=false;
        onReachCallback = OnReach;
        offset=_offset;
    }
    public void SetTargetBackToPlayer()
    {
        StartCoroutine(GameDataDNDL.Instance.DelayInvoke(2f, () =>
        {
            InputManager.Instance._cameraControle.SetTarget(GameDataDNDL.Instance.GetPlayer().gameObject, () => {
                GameDataDNDL.Instance.FTUT_Phase7();
            });
        }));
    }
}
public enum CameraTargetMode
{
    PlayerCam,FreeCam,BuildingMode
}
