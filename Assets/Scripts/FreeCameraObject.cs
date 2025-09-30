using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FreeCameraObject : MonoBehaviour
{
    public Transform SafeAreaMin, SafeAreaMax;
    
    public void onMove(Vector3 pos)
    {
        transform.position = new Vector3(SafeAreaMin.position.x <= Mathf.Abs(pos.x) && SafeAreaMax.position.x >= Mathf.Abs(pos.x) ? pos.x : transform.position.x, pos.y,
          SafeAreaMin.position.z <= Mathf.Abs(pos.z) && SafeAreaMax.position.z >= Mathf.Abs(pos.z) ? pos.z : transform.position.z);
    }
}
