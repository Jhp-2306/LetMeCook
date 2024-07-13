using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class check : MonoBehaviour
{
    HashSet<int> uni;
    //Hashtable table;
    int[] c = new int[5]{ 1, 2, 3, 3, 5 };
    private void Start()
    {
        uni = new HashSet<int>();
        foreach(var i in c)
        uni.Add(i);
        foreach (var t in uni) Debug.Log(t);
    }
}
