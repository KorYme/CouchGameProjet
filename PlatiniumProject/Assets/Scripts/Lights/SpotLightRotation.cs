using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering.Universal;

public class SpotLightRotation : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] float _speed = 1;
    [HideInInspector] public float firstLocationRotation, secondLocationRotation;

    public float FirstLocationRotation => firstLocationRotation + 90;
    public float SecondLocationRotation => secondLocationRotation + 90;

    private IEnumerator Start()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime * _speed;
            transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(firstLocationRotation, secondLocationRotation, (Mathf.Cos(timer) + 1) / 2));
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(FirstLocationRotation* Mathf.Deg2Rad), Mathf.Sin(FirstLocationRotation * Mathf.Deg2Rad),0) * 10f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(SecondLocationRotation * Mathf.Deg2Rad), Mathf.Sin(SecondLocationRotation * Mathf.Deg2Rad),0) * 10f);
    }
}