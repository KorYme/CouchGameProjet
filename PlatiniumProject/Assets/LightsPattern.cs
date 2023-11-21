using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsPattern : MonoBehaviour
{
    public enum Pattern
    {
        One_Two,
        FollowingDots,
    }

    [SerializeField] List<GameObject> _lights =  new List<GameObject>();
    [SerializeField] int _beatBeforeChangePattern;
    [Header("Following Dots Pattern")]
    [SerializeField, Range(1, 20f)] int _followingDots;

    int _beatCount;
    Pattern _currentState;
    Coroutine _patternCoroutine;

    One_Two_Pattern _one_Two_Pattern;
    

    private void Start()
    {
        _beatCount = 0;
        _currentState = Pattern.One_Two;
        Globals.BeatManager.OnBeatEvent.AddListener(() => UpdateValue());
    }

    private void UpdateValue()
    {
        _beatCount++;
        switch (_currentState)
        {
            case Pattern.One_Two:

                break;
            case Pattern.FollowingDots:

                break;
            default:
                break;
        }
        if (_beatCount < _beatBeforeChangePattern) return;
        _beatCount = 0;
        _currentState = (Pattern)(((int)_currentState++) % 2);
    }
}

public abstract class Pattern
{
    public abstract void UpdatePattern();
    public abstract bool IsEnlighted(int value);
}

public class One_Two_Pattern : Pattern
{
    int _even_Odd;
    public One_Two_Pattern()
    {
        _even_Odd = Random.Range(0,2);
    }

    public override void UpdatePattern() => _even_Odd = (_even_Odd + 1) % 2;

    public override bool IsEnlighted(int value) => value%2 == _even_Odd;
}

public class FollowingDots : Pattern
{
    int _followingDots;
    int _pointCount;
    int _currentDots;

    public FollowingDots(int followingDots, int numberOfDots)
    {
        _followingDots = followingDots;
        _pointCount = numberOfDots;
        _currentDots = Random.Range(0, numberOfDots);
    }

    public override bool IsEnlighted(int value) => (value % _pointCount) >= _currentDots || true;

    public override void UpdatePattern() => _currentDots = (_currentDots++)%_pointCount;
}