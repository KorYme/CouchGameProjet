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

    private IEnumerator Start()
    {
        _beatManager = Globals.BeatManager;
        yield return new WaitWhile(() => _beatManager.BeatDurationInMilliseconds == 0);
        float timer = 0f;
        float initialRadius = _light.pointLightOuterAngle;
        float radiusRatio = _light.pointLightOuterAngle == 0f ? 1f : _light.pointLightInnerAngle / _light.pointLightOuterAngle;
        while (true)
        {
            timer += Time.deltaTime * _Speed;
            _light.pointLightOuterAngle = initialRadius + _angleAddition * (Mathf.Cos(timer * Mathf.PI * _Speed) + 1f) / 2f;
            _light.pointLightInnerAngle = _light.pointLightOuterAngle * radiusRatio;
            yield return null;
        }
    }
}
