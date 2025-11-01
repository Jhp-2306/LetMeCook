using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using System.Xml.Schema;

public class DoorTrigger : MonoBehaviour
{

    public GameObject TriggerPoint;
    public GameObject MoveToPoint;

    public PlayableAreas ToArea;
    public DoorTrigger oppsiteTrigger;
    public GameObject DoorRotation;
    public AnimationCurve DoorRotationCurve;
    public float doorFinalRotationValue;
    Coroutine DoorAnimation;

    private void Start()
    {
       

    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ITriggerDoor>() != null)
        {

            oppsiteTrigger.gameObject.SetActive(false);
            //var starttimer=DateTime.Now;
            //var stopwatch =new System.Diagnostics.Stopwatch();
            ////stopwatch.Start();
            if (DoorAnimation != null)
                StopCoroutine(DoorAnimation);
            DoorAnimation =StartCoroutine(PlayDoorAnimation());
            col.gameObject.GetComponent<ITriggerDoor>().onDoorTrigger(MoveToPoint, ToArea, () =>
            {
                oppsiteTrigger.gameObject.SetActive(true);
                //var seconds=(starttimer-DateTime.Now).Seconds;
                //stopwatch.Stop();
                //Debug.Log(CustomLogs.CC_TagLog("DOOR Check", $"Total Timer of movement{stopwatch.ElapsedMilliseconds}mm"));

            });

        }
    }
    float Dooropeningtime = 1f;
    IEnumerator PlayDoorAnimation()
    {
        var time = 0f;
        while (time<= Dooropeningtime)
        {
            time += Time.deltaTime;
            var rot = DoorRotation.transform.eulerAngles;
            //var t = Vector3.Lerp(rot, new Vector3(rot.x,, rot.z));
            DoorRotation.transform.eulerAngles = new Vector3(rot.x, doorFinalRotationValue * DoorRotationCurve.Evaluate(time), rot.z);
            yield return null;

        }
        //yield return new WaitForSeconds(.5f);
        time = 0f;
        while (time <= Dooropeningtime)
        {
            time += Time.deltaTime;
            var rot = DoorRotation.transform.eulerAngles;
            //var t = Vector3.Lerp(rot, new Vector3(rot.x,, rot.z));
            DoorRotation.transform.eulerAngles = new Vector3(rot.x, doorFinalRotationValue - (doorFinalRotationValue * DoorRotationCurve.Evaluate(time)), rot.z);
            yield return null;

        }
        DoorAnimation = null;
    }

}
public interface ITriggerDoor
{
    public void onDoorTrigger(GameObject moveToPoint, PlayableAreas toArea, Action Callback);
}
