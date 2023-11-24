using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DontResizeWidthOnMovement : MonoBehaviour
{
    [SerializeField] Light2D _light2D;
    [SerializeField] Transform _target;
    //Distances
    float _originalDistance = 0f;
    float _currentDistance = 0f;
    //Angles
    Vector2 _originalSpotAngle;
    Vector2 _currentSpotAngle;
    void Start()
    {
        if (_target != null && _light2D != null)
        {
            _originalDistance = Vector3.Magnitude(_target.position - _light2D.transform.position);
            _currentDistance = _originalDistance;
            _originalSpotAngle = new Vector2(_light2D.pointLightInnerAngle,_light2D.pointLightOuterAngle);
            _currentSpotAngle = _originalSpotAngle;
            StartCoroutine(UpdateSpotAngle());
        }
    }

    IEnumerator UpdateSpotAngle()
    {
        while (true)
        {
            _currentDistance = Vector3.Magnitude(_target.position - _light2D.transform.position);
            _currentSpotAngle = _originalSpotAngle * _originalDistance / _currentDistance;
            _light2D.pointLightInnerAngle = _currentSpotAngle.x;
            _light2D.pointLightOuterAngle = _currentSpotAngle.y;
            yield return null;
        }
    }
}
