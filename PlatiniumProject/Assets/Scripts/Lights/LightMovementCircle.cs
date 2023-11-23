using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMovementCircle : MonoBehaviour
{
    //Circle movements
    [SerializeField, Range(0f, 10f)] float _circleRadius = 1f;
    [SerializeField, Range(0f, 1f)] float _speed = 1f, _timeOffset = 0f;

    ITimingable _beatManager;
    float _BeatSpeed => _beatManager == null ? 0f : 1000f / _beatManager.BeatDurationInMilliseconds;
    Vector3 _center;

    private void Start()
    {
        _center = transform.position;
        _beatManager = Globals.BeatManager;
        _beatManager.OnNextBeatStart += () => StartCoroutine(MovementCoroutine());
    }

    IEnumerator MovementCoroutine()
    {
        float timer = _BeatSpeed * _timeOffset * Mathf.PI * 2f * _speed;
        while (true)
        {
            timer += Time.deltaTime * Mathf.PI * 2f * _BeatSpeed;
            transform.position = _center + new Vector3(Mathf.Cos(timer), Mathf.Sin(timer)) * _circleRadius;
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Application.isPlaying ? _center : transform.position, _circleRadius);
    }
}
