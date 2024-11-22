using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameConsoleSlider : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public void SetText(string text)
    {
        Text.text = text;
    }
}
