using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conversions
{
public class Converions : MonoBehaviour
{
   public static Vector3Int Vector3DFloatToInt(Vector3 val)
    {
        return new Vector3Int((int)val.x, (int)val.y, (int)val.z);
    }
    public static Vector2Int Vector2DFloatToInt(Vector2 val)
    {
        return new Vector2Int((int)val.x, (int)val.y);
    }

    public static DateTime StringToDateTime(string str)
    {
        return DateTime.ParseExact(str, Constants.Constant.DateTimeFormate,null);
    }
}
}
