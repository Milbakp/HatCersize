using System;
// using System.Runtime.Serialization;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private float tileLockWidth = 10.0f;
    public void Update()
    {
        transform.localPosition = new Vector3(RoundToNearestMultiple(transform.localPosition.x, tileLockWidth
        ), transform.localPosition.y, RoundToNearestMultiple(transform.localPosition.z, tileLockWidth));
    }

    public static float RoundToNearestMultiple(float value, float multiple)
    {
        if (multiple == 0)
        {
            return value;
        }

        // Divide the number by the multiple, round it, and multiply back by the multiple
        return (float)Math.Round(value / multiple) * multiple;
    }
    public void tileLock()
    {
        transform.localPosition = new Vector3(RoundToNearestMultiple(transform.localPosition.x, tileLockWidth
        ), transform.localPosition.y, RoundToNearestMultiple(transform.localPosition.z, tileLockWidth));
    }
}
