using System.Collections.Generic;
using UnityEngine;
using Constants;


[CreateAssetMenu(fileName = "PerkSystem", menuName = "ScriptableObject/PerkSystem")]
public class SO_PerkSystem : ScriptableObject
{
    [SerializeField]
    public List<PerkData> perkData;
}
