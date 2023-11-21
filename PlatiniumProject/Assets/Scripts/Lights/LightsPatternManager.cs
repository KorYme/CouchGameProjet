using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsPatternManager : MonoBehaviour
{
    public enum PATTERN_TYPE
    {
        ONE_TWO,
        FOLLOWING_DOTS,
    }

    [SerializeField] List<GameObject> _lights =  new List<GameObject>();
    [SerializeField, Range(1, 50f)] int _beatBeforeChangePattern;
    [Header("Following Dots Pattern")]
    [SerializeField, Range(1, 9f)] int _followingDots;

    int _beatCount;
    PATTERN_TYPE _currentState;
    Coroutine _patternCoroutine;

    Dictionary<PATTERN_TYPE, LightsPattern> _allPatterns = new();
    

    private void Start()
    {
        _beatCount = 0;
        _currentState = PATTERN_TYPE.ONE_TWO;
        _allPatterns.Add(PATTERN_TYPE.ONE_TWO, new One_Two_Pattern());
        _allPatterns.Add(PATTERN_TYPE.FOLLOWING_DOTS, new FollowingDots(_followingDots, _lights.Count));
        UpdateLights();
        Globals.BeatManager.OnBeatEvent.AddListener(() => UpdateValue());
    }

    private void UpdateValue()
    {
        UpdateLights();
        _beatCount++;
        if (_beatCount < _beatBeforeChangePattern) return;
        _beatCount = 0;
        _currentState = (PATTERN_TYPE)((((int)_currentState) + 1) % 2);
        Debug.Log($"Changement de state : {_currentState}");
    }

    private void UpdateLights()
    {
        LightsPattern currentPattern = _allPatterns[_currentState];
        currentPattern.UpdatePattern();
        for (int i = 0; i < _lights.Count; i++)
        {
            _lights[i].SetActive(currentPattern.IsThisLightEnlighted(i));
        }
    }
}

