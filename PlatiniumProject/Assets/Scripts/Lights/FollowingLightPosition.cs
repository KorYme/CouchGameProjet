using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingLightPosition : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float _timeToReachPosition = .2f;
    [SerializeField] Vector2 _offset = Vector2.up;

    Vector3 _velocity;

    private IEnumerator Start()
    {
        _velocity = Vector3.zero;
        while (true)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _target.position + (Vector3)_offset, ref _velocity, _timeToReachPosition);
            yield return null;
        }
    }
}
