using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatSign : MonoBehaviour
{
    private float _currentBeatDuration;
    [SerializeField] private Slider _slider;
    private float _currenSliderValue; 

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
        //Globals.BeatManager.onne
        _currentBeatDuration = Globals.BeatManager.BeatDurationInMilliseconds / 1000;
        Debug.Log(Globals.BeatManager.BeatDurationInMilliseconds);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentSliderValue = Mathf.PingPong(Time.deltaTime, _currentBeatDuration) / _currentBeatDuration;
        Debug.Log(_currentBeatDuration);
    }
}
