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

    private void Reset()
    {
        SetUpLights();
    }
    public void SetUpLights() => _lights2D = GetComponentsInChildren<Light2D>(false);


    private void Start()
    {
        if (_lightColorData.onBeatColorList.Count == 0 || _lights2D.Length == 0) return;
        Globals.BeatManager.OnBeatEvent.AddListener(ChangeToRandomColor);
        Globals.DropManager.OnDropSuccess += OnDropSuccessEffect;
        Globals.DropManager.OnDropFail += OnDropFailEffect;
        Globals.DropManager.OnDropEnded += () => Globals.BeatManager.OnBeatEvent.AddListener(ChangeToRandomColor);
    }

    private void OnDestroy()
    {
        if (_lightColorData.onBeatColorList.Count == 0 || _lights2D.Length == 0) return;
        Globals.BeatManager.OnBeatEvent.RemoveListener(ChangeToRandomColor);
        Globals.DropManager.OnDropSuccess -= OnDropSuccessEffect;
        Globals.DropManager.OnDropFail -= OnDropFailEffect;
        Globals.DropManager.OnDropEnded -= () => Globals.BeatManager.OnBeatEvent.AddListener(ChangeToRandomColor);
    }

    private void ChangeToRandomColor() => ChangeAllLightsColor(_lightColorData.onBeatColorList[UnityEngine.Random.Range(0, _lightColorData.onBeatColorList.Count)]);

    private void OnDropSuccessEffect()
    {
        Globals.BeatManager.OnBeatEvent.RemoveListener(ChangeToRandomColor);
        ChangeAllLightsColor(_lightColorData.onDropSuccessColor);
    }

    private void OnDropFailEffect()
    {
        Globals.BeatManager.OnBeatEvent.RemoveListener(ChangeToRandomColor);
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
