
using System.Collections.Generic;
using UnityEngine;

public class Pc_StatsPage : MonoBehaviour
{
    [System.Serializable]
    public struct stat_slider
    {
        public TMPro.TextMeshProUGUI m_Title;
        public TMPro.TextMeshProUGUI m_Value;
    }
    [SerializeField]private List<stat_slider> m_Sliders;
    public List<GameObject> Sliders;
    public void init()
    {
        int idx = 0;
        m_Sliders = new List<stat_slider>();
        foreach (var slider in Sliders)
        {
            stat_slider stat = new stat_slider();
            stat.m_Title = slider.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            stat.m_Value = slider.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
            m_Sliders.Add(stat);
        }
        foreach (var s in m_Sliders)
        {
            idx++;
            var t = StatsManager.Instance.GetStats(idx);
            s.m_Title.text = t.Descriptions;
            s.m_Value.text = t.GetValue();
        }
    }
}
