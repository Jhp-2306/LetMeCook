using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public TMPro.TextMeshProUGUI DigiClock;
    public GameObject HrsHand, MinHand;
    [SerializeField] float animationtimer = 1.3f;
    [SerializeField] GameObject parentObj;
    bool isexpand;
    public Vector3 InitPosition, FinalPosition;
    void Update()
    {
        DigiClock.text = $"Day-{TimeManagementDNDL.Instance.GetTotalDays()+1}";
        var hrshandRotation = (TimeManagementDNDL.Instance.GetHrs() / 12) * 360;
        HrsHand.transform.eulerAngles = new Vector3(0, 0, -hrshandRotation);
        var minshandRotation = (TimeManagementDNDL.Instance.GetMins() / 60) * 360;
        MinHand.transform.eulerAngles = new Vector3(0, 0, -minshandRotation);
    }
    public void OnClick()
    {
        //if (expandCoroutine != null) StopCoroutine(expandCoroutine);
        parentObj.GetComponent<RectTransform>().localPosition = isexpand? FinalPosition:InitPosition;
        //Debug.Log(parentObj.GetComponent<RectTransform>().localPosition);
        isexpand = !isexpand;
    }
     
    

}
