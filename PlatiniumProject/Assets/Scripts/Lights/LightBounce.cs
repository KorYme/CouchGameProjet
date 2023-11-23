using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class LightBounce : MonoBehaviour
{
    ITimingable _beatManager;
    float _Speed => _beatManager == null ? 0f : 1000f / _beatManager.BeatDurationInMilliseconds;

    [SerializeField] Light2D _light;
    [SerializeField] float _angleAddition;

    private void Reset()
    {
        _light = GetComponent<Light2D>();
    }

    private void Start()
    {
        _beatManager = Globals.BeatManager;
        _beatManager.OnNextBeat += () => StartCoroutine(BouncingLight());
    }

    IEnumerator BouncingLight()
    {
        float timer = 0f;
        float initialAngle = _light.pointLightOuterAngle;
        float angleRatio = _light.pointLightOuterAngle == 0f ? 1f : _light.pointLightInnerAngle / _light.pointLightOuterAngle;
        while (true)
        {
            timer += Time.deltaTime * _Speed;
            _light.pointLightOuterAngle = initialAngle + _angleAddition * (Mathf.Sin(timer * Mathf.PI * 2f) + 1f) / 2f;
            _light.pointLightInnerAngle = _light.pointLightOuterAngle * angleRatio;
            yield return null;
        }
    }
}
