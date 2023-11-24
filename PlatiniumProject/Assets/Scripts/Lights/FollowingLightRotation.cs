using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FollowingLightRotation : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float _damping = .5f;
    [SerializeField] Light2D _light2D;
    [SerializeField] bool _isResizable = true;
    [SerializeField, Range(0f, 10f)] float _radiusAdded = 0f;

    Vector3 _velocity;
    float _outerInnerPercent;

    private void Reset()
    {
        _light2D = GetComponent<Light2D>();
    }

    private IEnumerator Start()
    {
        if (_isResizable && _light2D.pointLightOuterRadius != 0)
        {
            _outerInnerPercent = _light2D.pointLightInnerRadius / _light2D.pointLightOuterRadius;
        }
        _velocity = Vector3.zero;
        while (true)
        {
            Vector3 relative = transform.InverseTransformPoint(_target.position);
            transform.Rotate(0, 0, Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg - 90f);
            if (_isResizable)
            {
                _light2D.pointLightOuterRadius = Vector2.Distance(_target.position, transform.position) + _radiusAdded;
                _light2D.pointLightInnerRadius = _light2D.pointLightOuterRadius * _outerInnerPercent;
            }
            //transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, new(0, 0, Mathf.Atan2((_target.position - transform.position).y, (_target.position - transform.position).x) * Mathf.Rad2Deg - 90), ref _velocity, _damping);
            yield return null;
        }
    }
}
