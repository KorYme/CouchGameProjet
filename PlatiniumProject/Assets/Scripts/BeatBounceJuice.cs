using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BeatBounceJuice : MonoBehaviour
{
    [SerializeField] private BeatBounceJuiceData _data;
    private BeatBounceJuiceData.PhaseData _currentPhaseData;
    private Coroutine _beatRoutine;
    private Vector3 _initScale;
    private int _phaseIndex = -1;

    private void Start()
    {
        Globals.BeatManager.OnBeatEvent.AddListener(OnBeat);
        Globals.DropManager.OnDropSuccess += LoadPhaseData;
        Globals.DropManager.OnDropFail += LoadPhaseData;
        _initScale = transform.localScale;
        LoadPhaseData();
    }

    private void LoadPhaseData()
    {
        _phaseIndex++;
        _currentPhaseData = _data.phaseData[_phaseIndex];
    }

    private void OnDestroy()
    {
        Globals.BeatManager.OnBeatEvent.RemoveListener(OnBeat);
        Globals.DropManager.OnDropSuccess -= LoadPhaseData;
        Globals.DropManager.OnDropFail -= LoadPhaseData;
    }

    private void OnBeat()
    {
        if(!_currentPhaseData.usedThisPhase)
            return;
        
        if (_beatRoutine != null)
        {
            StopCoroutine(_beatRoutine);
        }
        _beatRoutine = StartCoroutine(BeatRoutine());
    }

    IEnumerator BeatRoutine()
    {
        float duration = Globals.BeatManager.BeatDurationInSeconds * _currentPhaseData.beatPercentage;
        float timer = 0f;
        float percentage = 0f;
        transform.localScale = _initScale;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            percentage = _data.bounceCurve.Evaluate(timer / duration);
            transform.localScale = Vector3.Lerp(_initScale, _initScale * _currentPhaseData.maxScale, percentage);
            yield return null;
        }

        transform.localScale = _initScale;
        _beatRoutine = null;
    }
}
