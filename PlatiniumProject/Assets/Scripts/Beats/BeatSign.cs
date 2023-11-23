using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatSign : MonoBehaviour
{
    public enum DISPLAY_STYLE
    {
        EDGE_TO_CENTER_INCREMENTAL,   
        EDGE_TO_CENTER_DECREMENTAL,   
        CENTER_TO_EDGE_INCREMENTAL,   
        CENTER_TO_EDGE_DECREMENTAL,   
    }
    private float _currentBeatDuration;
    [SerializeField] private DISPLAY_STYLE _displayStyle;
    [SerializeField] private Slider _sliderLeft;
    [SerializeField] private RectTransform _fillLeft;
    [SerializeField] private Slider _sliderRight;
    [SerializeField] private RectTransform _fillRight;
    private float _currenSliderValue;
    private float _pingPong;
    
    private float CurrentSliderValue
    {
        get
        {
            return _currenSliderValue;
        }
        set
        {
            _sliderLeft.value = value;
            _sliderRight.value = value;
            _currenSliderValue = value;
        }
    }
    void Start()
    {
        Globals.BeatManager.OnNextBeat += () => StartBeat();

    }
    
    private void StartBeat()
    {
        _currentBeatDuration = Globals.BeatManager.BeatDurationInMilliseconds / 1000f;
        _sliderLeft.maxValue = _currentBeatDuration;
        _sliderRight.maxValue = _currentBeatDuration;
        switch (_displayStyle)
        {
            case DISPLAY_STYLE.EDGE_TO_CENTER_INCREMENTAL:
                _sliderRight.transform.localScale = new Vector3(-_sliderRight.transform.localScale.x,_sliderLeft.transform.localScale.y, _sliderLeft.transform.localScale.z);
                StartCoroutine(SliderIncrementalRoutine());
                break;
            case DISPLAY_STYLE.EDGE_TO_CENTER_DECREMENTAL:
                _sliderLeft.transform.localScale = new Vector3(-_sliderLeft.transform.localScale.x,_sliderLeft.transform.localScale.y, _sliderLeft.transform.localScale.z);
                _fillLeft.localScale = new Vector3(-_fillLeft.localScale.x,_fillLeft.localScale.y, _fillLeft.localScale.z);
                _fillRight.localScale = new Vector3(-_fillRight.localScale.x,_fillRight.localScale.y, _fillRight.localScale.z);
                StartCoroutine(SliderDecrementalRoutine());
                break;
            case DISPLAY_STYLE.CENTER_TO_EDGE_INCREMENTAL:
                _sliderLeft.transform.localScale = new Vector3(-_sliderLeft.transform.localScale.x,_sliderLeft.transform.localScale.y, _sliderLeft.transform.localScale.z);
                _fillLeft.localScale = new Vector3(-_fillLeft.localScale.x,_fillLeft.localScale.y, _fillLeft.localScale.z);
                _fillRight.localScale = new Vector3(-_fillRight.localScale.x,_fillRight.localScale.y, _fillRight.localScale.z);
                StartCoroutine(SliderIncrementalRoutine());
                break;
            case DISPLAY_STYLE.CENTER_TO_EDGE_DECREMENTAL:
                _sliderRight.transform.localScale = new Vector3(-_sliderRight.transform.localScale.x,_sliderLeft.transform.localScale.y, _sliderLeft.transform.localScale.z);
                StartCoroutine(SliderDecrementalRoutine());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator SliderIncrementalRoutine()
    {
        while (true)
        {
            _pingPong = Mathf.Repeat( Time.time,_currentBeatDuration);
            CurrentSliderValue = Mathf.Lerp(0f, _currentBeatDuration, _pingPong / _currentBeatDuration);
            //Debug.Log();
            yield return null;
        }
    }

    private IEnumerator SliderDecrementalRoutine()
    {
        while (true)
        {
            _pingPong = Mathf.Repeat( Time.time, _currentBeatDuration);
            CurrentSliderValue = _currentBeatDuration  - Mathf.Lerp(0f, _currentBeatDuration, _pingPong / _currentBeatDuration);
            yield return null;
        }
    }
}
