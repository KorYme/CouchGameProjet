using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BarmanPosition : MonoBehaviour
{
    [SerializeField] float _radius;

    #if UNITY_EDITOR 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, _radius);
    }
    #endif
}
