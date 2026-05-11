using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pc_Market_Details", menuName = "ScriptableObject/Pc_Market")]
public class SO_PCMarketItemList : ScriptableObject
{
    public List<Pc_MarketItemsDetails> items;

}
