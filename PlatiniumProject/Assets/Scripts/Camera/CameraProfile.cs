using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CAMERA_TYPE
{
    ENTERY,
    BOUNCER,
    BARMAN,
    DJ,
    DANCEFLOOR
}
public class CameraProfile : MonoBehaviour
{
    [SerializeField] private CameraProfileData _profileData;
    [SerializeField] private CAMERA_TYPE _cameraType;

    private bool canDezoom;

    public CAMERA_TYPE CameraType => _cameraType;
    
    private Camera _cam;
    private Vector3 _initPos;
    private Vector3 _offset;
    private float _initSize;
    private Transform _target;
    
    [Header("Coroutines")]
    private Coroutine _shakeRoutine;
    private Coroutine _moveRoutine;
    private Coroutine _zoomRoutine;
    private Coroutine _followMoveRoutine;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _initPos = transform.position;
        _initSize = _cam.orthographicSize;
    }

    private void Update()
    {
        Debug.Log(_initPos);
    }

    private void Start()
    {
        StartCoroutine(FollowRoutine());
    }

    #region Shake
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
    #endregion

    #region Zoom

    public void StartFocus(float duration, float percentage, Transform target)
    {
        _zoomRoutine = StartCoroutine(ZoomRoutine(duration, percentage, target));
        _target = target;
    }

    public void StopFocus()
    {
        canDezoom = true;
    }

    IEnumerator ZoomRoutine(float duration, float percentage, Transform target)
    {
        float timer = 0;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            _cam.orthographicSize = Mathf.Lerp(_initSize, _initSize * percentage, timer / duration);    
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitUntil(() => canDezoom);
        _target = null;
        StartCoroutine(FollowMoveRoutine());
        canDezoom = false;
        timer = 0;
        float size = _cam.orthographicSize;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            _cam.orthographicSize = Mathf.Lerp(size, _initSize, timer / duration);    
            yield return new WaitForEndOfFrame();
        }

        _zoomRoutine = null;
    }

    IEnumerator FollowRoutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => _target != null);
            if (_followMoveRoutine == null)
            {
                _followMoveRoutine = StartCoroutine(FollowMoveRoutine());
            }
            yield return null;
        }
    }

    IEnumerator FollowMoveRoutine()
    {
        float timer = 0;
        Vector3 pos = transform.localPosition;
        Vector3 target;
        
        target = _target != null ? _target.position : _initPos;
        
        while (timer < _profileData.snapDuration)
        {
            timer += Time.deltaTime; 
            float percentage = _profileData.snapCurve.Evaluate(timer / _profileData.snapDuration);
            transform.localPosition = Vector3.Lerp(pos, target, percentage);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10);
            yield return new WaitForEndOfFrame();
        }

        _followMoveRoutine = null;
    }
    

    #endregion

}
