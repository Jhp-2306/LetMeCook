using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public TMPro.TextMeshProUGUI DigiClock;
    void Update()
    {
        DigiClock.text = TimeManagementDNDL.Instance.GetTime(false);
    }
}
