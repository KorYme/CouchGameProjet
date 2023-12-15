using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering.Universal;

public class SpotLightRotation : MonoBehaviour
{
    [HideInInspector] public float firstLocationRotation, secondLocationRotation;

    public float FirstLocationRotation => firstLocationRotation + 90;
    public float SecondLocationRotation => secondLocationRotation + 90;

    float _Speed => _beatManager == null ? 0f : 1000f / _beatManager.BeatDurationInMilliseconds;
    ITimingable _beatManager;

    private IEnumerator Start()
    {
        _beatManager = Globals.BeatManager;
        float timer = 0f;
        yield return new WaitWhile(() => _beatManager.BeatDurationInMilliseconds == 0);
        while (true)
        {
            timer += Time.deltaTime * _Speed;
            transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(firstLocationRotation, secondLocationRotation, (Mathf.Cos(timer * Mathf.PI) + 1) / 2));
            yield return new WaitUntil(() => Globals.BeatManager?.IsPlaying ?? true);
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