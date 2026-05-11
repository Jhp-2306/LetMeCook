using UnityEngine;

public class Pc_Checkout_Slider : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Label;
    public TMPro.TextMeshProUGUI Count;

    public void SetCheckout(string label,string count) {     
        Label.text = label;
        Count.text = count; 
        this.gameObject.SetActive(true);
    }
}
