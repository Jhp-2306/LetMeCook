using System.Collections.Generic;
using UnityEngine;


public class Pc_UpgradeWindow : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject Parent;
    public Dictionary<string, Pc_UpgradeWindow_Slider> _items;
    public void init()
    {
        if (_items == null)
            _items = new Dictionary<string, Pc_UpgradeWindow_Slider>();
        var items = GameDataDNDL.Instance.GetalltheLevels();
        if (items == null) return;
        if (_items.Count != _items.Count)
            foreach (var t in items)
            {
                var go = Instantiate(Prefab, Parent.transform);
                //var sprite = AssetLoader.Instance.GetIcons(t.GetIcon());
                go.GetComponent<Pc_UpgradeWindow_Slider>().SetUp(t.Value.Name, t.Value.Level, t.Key);
                _items.Add(t.Key, go.GetComponent<Pc_UpgradeWindow_Slider>());
            }
        else
            foreach (var t in items)
            {
                //if()
                if (!_items.ContainsKey(t.Key))
                {
                    var go = Instantiate(Prefab, Parent.transform);
                    //var sprite = AssetLoader.Instance.GetIcons(t.GetIcon());
                    go.GetComponent<Pc_UpgradeWindow_Slider>().SetUp(t.Value.Name, t.Value.Level, t.Key);
                    _items.Add(t.Key, go.GetComponent<Pc_UpgradeWindow_Slider>());
                }
            }
    }
}
