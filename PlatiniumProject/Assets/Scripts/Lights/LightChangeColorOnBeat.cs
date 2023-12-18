using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class LightChangeColorOnBeat : MonoBehaviour
{
    [SerializeField] Light2D[] _lights2D;
    [SerializeField] LightColorData _lightColorData;
    UnityAction _currentAction;

    private void Reset()
    {
        SetUpLights();
    }
    public void SetUpLights() => _lights2D = GetComponentsInChildren<Light2D>(false);


    private void Start()
    {
        _currentAction = ChangeToRandomColor;
        Globals.DropManager.OnDropSuccess += OnDropSuccessEffect;
        Globals.DropManager.OnDropFail += OnDropFailEffect;
        Globals.DropManager.OnDropEnded += () => _currentAction = ChangeToRandomColor;
        if (_lightColorData.onBeatColorList.Count == 0 || _lights2D.Length == 0) return;
        Globals.BeatManager.OnBeatEvent.AddListener(_currentAction);
    }

    private void OnDestroy()
    {
        Globals.DropManager.OnDropSuccess -= OnDropSuccessEffect;
        Globals.DropManager.OnDropFail -= OnDropFailEffect;
        Globals.DropManager.OnDropEnded -= () => _currentAction = ChangeToRandomColor;
        if (_lightColorData.onBeatColorList.Count == 0 || _lights2D.Length == 0) return;
        Globals.BeatManager?.OnBeatEvent.RemoveListener(_currentAction);
    }

    private void ChangeToRandomColor() => ChangeAllLightsColor(_lightColorData.onBeatColorList[UnityEngine.Random.Range(0, _lightColorData.onBeatColorList.Count)]);

    private void OnDropSuccessEffect()
    {
        _currentAction = null;
        ChangeAllLightsColor(_lightColorData.onDropSuccessColor);
    }

    private void OnDropFailEffect()
    {
        _currentAction = null;
        ChangeAllLightsColor(_lightColorData.onDropFailedColor);
    }

    void ChangeAllLightsColor(Color color)
    {
        foreach (Light2D light in _lights2D)
        {
            if (light == null) continue;
            light.color = color;
        }
    }
}
