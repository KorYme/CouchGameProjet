using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoints : MonoBehaviour
{
    [SerializeField] private Transform[] _exitsPoints;

    private void Awake()
    {
        Globals.ExitPoints ??= this;
    }

    public Vector3 FindClosestExitPoint(Vector3 position)
    {
        Vector3 result = Vector3.zero;
        float minDist = 0f;
        float currentDist = 0f;
        foreach (var point in _exitsPoints)
        {
            currentDist = Vector3.Distance(position, point.position);
            if (minDist == 0f)
                minDist = currentDist;

            if (currentDist < minDist)
            {
                minDist = currentDist;
                result = point.position;
            }
        }

        return result;
    }
}
