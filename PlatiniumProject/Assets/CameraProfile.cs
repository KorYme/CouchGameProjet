using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraProfile : MonoBehaviour
{
    private Coroutine _shakeRoutine;
    private Coroutine _moveRoutine;
    private Vector3 _initPos;
    private Vector3 _offset;

    private void Awake()
    {
        _initPos = transform.position;
    }

    public void StartShake(float duration, float intensity, float speed)
    {
        _shakeRoutine = StartCoroutine(ShakeRoutine(duration, intensity, speed));
    }

    IEnumerator ShakeRoutine(float duration, float intensity, float speed)
    {
        float timer = 0;
        while (timer < duration)
        {
            _offset = Vector3.one + new Vector3(Random.Range(-intensity,intensity+1),Random.Range(-intensity,intensity+1),0);
            _moveRoutine = StartCoroutine(MoveRoutine(speed));
            yield return new WaitUntil(() => _moveRoutine == null);
            timer += speed;
        }

        transform.position = _initPos;
        _shakeRoutine = null;
    }

    IEnumerator MoveRoutine(float moveDuration)
    {
        float timer = 0;
        float pingPong = 0;
        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            pingPong = Mathf.PingPong(Time.time, moveDuration / 2);
            transform.position = Vector3.Lerp(_initPos, _initPos + _offset, pingPong);
            yield return new WaitForEndOfFrame();
        }
        _moveRoutine = null;
    }

}
