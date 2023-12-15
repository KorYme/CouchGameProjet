using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightBlinking : MonoBehaviour
{
    [SerializeField] Light2D _light;
    [SerializeField] float _radiusAddition;

    ITimingable _beatManager;
    float _Speed => _beatManager == null ? 0f : 1000f / _beatManager.BeatDurationInMilliseconds;


    private void Reset()
    {
        _light = GetComponent<Light2D>();
    }

    private void Start()
    {
        _beatManager = Globals.BeatManager;
        _beatManager.OnNextBeat += () => StartCoroutine(BlinkingLight());
    }

    IEnumerator BlinkingLight()
    {
        float timer = 0f;
        float initialRadius = _light.pointLightOuterRadius;
        float radiusRatio = _light.pointLightOuterRadius == 0f ? 1f : _light.pointLightInnerRadius / _light.pointLightOuterRadius;
        while (true)
        {
            timer += Time.deltaTime * _Speed;
            _light.pointLightOuterRadius = initialRadius + _radiusAddition * (Mathf.Sin(timer * Mathf.PI * 2f) + 1f) / 2f;
            _light.pointLightInnerRadius = _light.pointLightOuterRadius * radiusRatio;
            yield return new WaitUntil(() => Globals.BeatManager?.IsPlaying ?? true);
        }
    }
}
