using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightIntensityTrigger : MonoBehaviour
{
    public static event Action TurnOffLights;
    public static event Action TurnOnLights;

    [SerializeField] Light2D _light2D;

    float _initialIntensity;

    private void Reset()
    {
        _light2D = GetComponent<Light2D>();
    }

    private void Awake()
    {
        _initialIntensity = _light2D.intensity;
        TurnOnLights += TurnOnLight;
        TurnOffLights += TurnOffLight;
    }

    private void OnDestroy()
    {
        TurnOnLights -= TurnOnLight;
        TurnOffLights -= TurnOffLight;
    }

    void TurnOnLight()
    {
        _light2D.intensity = _initialIntensity;
    }

    void TurnOffLight()
    {
        _light2D.intensity = .0f;
    }
}
