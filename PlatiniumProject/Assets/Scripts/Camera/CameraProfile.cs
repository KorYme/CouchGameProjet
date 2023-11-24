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
    private float _zoomOffset;
    private float _initSize;
    private float _currentInitSize;
    private Transform _target;
    
    [Header("Coroutines")]
    private Coroutine _shakeRoutine;
    private Coroutine _moveRoutine;
    private Coroutine _zoomRoutine;
    private Coroutine _pulseRoutine;
    private Coroutine _followMoveRoutine;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _initPos = transform.position;
        _initSize = _cam.orthographicSize;
    }
    private void Start()
    {
        StartCoroutine(FollowRoutine());
        StartPulseZoom();
        _currentInitSize = _initSize;
        //StartShake();   
    }

    private void LateUpdate()
    {
        _zoomOffset = 0;
    }

    #region Shake
    public void StartShake(bool isInfinite = true)
    {
        if (_shakeRoutine != null)
        {
            StopCoroutine(_shakeRoutine);
            _shakeRoutine = null;
            transform.position = _initPos;
        }

        _shakeRoutine = StartCoroutine(ShakeRoutine(isInfinite, _profileData.shakeDuration, _profileData.shakeIntensity, _profileData.shakeSpeed));
    }
    
    public void StopShake()
    {
        StopCoroutine(_shakeRoutine);
        _shakeRoutine = null;
        transform.position = _initPos;
    }
    IEnumerator ShakeRoutine(bool isIninite, float duration, float intensity, float speed)
    {
        float timer = 0;
        while (timer < duration)
        {
            _offset = Vector3.one + new Vector3(Random.Range(-intensity,intensity+1),Random.Range(-intensity,intensity+1),0);
            _moveRoutine = StartCoroutine(MoveRoutine(speed));
            yield return new WaitUntil(() => _moveRoutine == null);
            
            if(!isIninite)
            {
                timer += speed;
            }
            
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

    public void StartFocus(Transform target)
    {
        _zoomRoutine = StartCoroutine(ZoomRoutine(_profileData.focusDuration, _profileData.focusPercentage, target));
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
            _currentInitSize = Mathf.Lerp(_initSize, _initSize * percentage, timer / duration);
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
            _currentInitSize = Mathf.Lerp(size, _initSize, timer / duration); 
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
        
        
        while (timer < _profileData.snapDuration)
        {
            target = _target != null ? _target.position : _initPos;
            timer += Time.deltaTime; 
            float percentage = _profileData.snapCurve.Evaluate(timer / _profileData.snapDuration);
            transform.localPosition = Vector3.Lerp(pos, target, percentage);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10);
            yield return new WaitForEndOfFrame();
        }

        _followMoveRoutine = null;
    }
    

    #endregion

    #region Pulse

    public void StartPulseZoom()
    {
        if (_pulseRoutine != null)
        {
            StopPulseZoom();
        }
        _pulseRoutine = StartCoroutine(BeatPulseZoom(_profileData.pulsePercentage));
    }
    
    public void StopPulseZoom()
    {
        StopCoroutine(_pulseRoutine);
        _pulseRoutine = null;
    }
    
    IEnumerator BeatPulseZoom(float percentage)
    {
        float pingPong;
        while (true)
        {
            pingPong = Mathf.PingPong(Time.time, (Globals.BeatManager.BeatDurationInMilliseconds / 2000f));
            pingPong = _profileData.pulseCurve.Evaluate(pingPong);
            _cam.orthographicSize = Mathf.Lerp(_currentInitSize, _currentInitSize * percentage, pingPong);
            yield return new WaitForEndOfFrame();
            //yield return new WaitUntil(() => _zoomRoutine == null);
        }
    }

    #endregion

}
