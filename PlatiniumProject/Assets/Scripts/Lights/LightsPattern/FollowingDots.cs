using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowingDots : LightsPattern
{
    int _followingDots;
    int _pointCount;
    int _currentDots;

    public FollowingDots(int followingDots, int numberOfDots)
    {
        _followingDots = followingDots;
        _pointCount = numberOfDots / 4;
        _currentDots = Random.Range(0, numberOfDots);
    }

    public override bool IsThisLightEnlighted(int index)
    {
        int newIndex = index % _pointCount;
        return (newIndex >= _currentDots && newIndex < _currentDots + _followingDots)
            || (newIndex >= _currentDots - _pointCount && newIndex < _currentDots + _followingDots - _pointCount);
    }

    public override void UpdatePattern() => _currentDots = (++_currentDots) % _pointCount;
}