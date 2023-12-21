using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightIntensityTrigger : MonoBehaviour
{
    public static event Action TurnOffLights;
    public static event Action TurnOnLights;

    [Header("IntensityParameters"), SerializeField] Light2D _light2D;
    [SerializeField] LightIntensityData _dropSuccessData, _dropFailedData;
    float _lightRecoveryTime;

    Coroutine _intensityCoroutine;
    float _initialIntensity;

    public static void ActivateLight(bool isOn) => (isOn ? TurnOnLights : TurnOffLights)?.Invoke();

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

    private void Start()
    {
        if (_dropSuccessData == null || _dropFailedData == null) return;
        Globals.DropManager.OnDropSuccess += () => _intensityCoroutine = StartCoroutine(DropCoroutine(_dropSuccessData));
        Globals.DropManager.OnDropFail += () => _intensityCoroutine = StartCoroutine(DropCoroutine(_dropFailedData));
        Globals.DropManager.OnDropEnded += StopBehaviour;
    }

    private void OnDestroy()
    {
        TurnOnLights -= TurnOnLight;
        TurnOffLights -= TurnOffLight;
        if (_dropSuccessData == null || _dropFailedData == null) return;
        Globals.DropManager.OnDropSuccess -= () => _intensityCoroutine = StartCoroutine(DropCoroutine(_dropSuccessData));
        Globals.DropManager.OnDropFail -= () => _intensityCoroutine = StartCoroutine(DropCoroutine(_dropFailedData));
        Globals.DropManager.OnDropEnded -= StopBehaviour;
    }

    void TurnOnLight()
    {
        if (_intensityCoroutine != null) return;
        _light2D.intensity = _initialIntensity;
    }

    void TurnOffLight()
    {
        if (_intensityCoroutine != null) return;
        _light2D.intensity = .0f;
    }

    IEnumerator DropCoroutine(LightIntensityData data)
    {
        _lightRecoveryTime = data.lightRecoveryTime;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime / 3.5f;
            _light2D.intensity = data.dropLoopCurve.Evaluate(timer) * _initialIntensity * data.curveMultiplier;
            yield return new WaitUntil(() => Globals.BeatManager?.IsPlaying ?? true);
        }
    }

    private void StopBehaviour()
    {
        if (_intensityCoroutine != null)
        {
            StopCoroutine(_intensityCoroutine);
            _intensityCoroutine = null;
        }
        StartCoroutine(GoBackToNormal());
    }
    
    IEnumerator GoBackToNormal()
    {
        while (_light2D.intensity < _initialIntensity)
        {
            _light2D.intensity += Time.deltaTime / _lightRecoveryTime;
            yield return new WaitUntil(() => Globals.BeatManager?.IsPlaying ?? true);
        }
        _light2D.intensity = _initialIntensity;
    }
}
