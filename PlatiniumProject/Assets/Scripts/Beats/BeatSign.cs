using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatSign : MonoBehaviour
{
    private float _currentBeatDuration;
    [SerializeField] private Slider _slider;
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
            _slider.value = value;
            _currenSliderValue = value;
        }
    }
    void Start()
    {
        Globals.BeatManager.OnNextBeat += () => StartCoroutine(Routine());

    }
    
    IEnumerator Routine()
    {
        _currentBeatDuration = Globals.BeatManager.BeatDurationInMilliseconds / 1000f;
        _slider.maxValue = _currentBeatDuration / 2;
        while (true)
        {
            _pingPong = Mathf.Repeat( Time.time, _currentBeatDuration);
            CurrentSliderValue = Mathf.Lerp(0f, _currentBeatDuration, _pingPong);
            yield return null;
        }
    }
}
