using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _timer;
    private int _timerValue = 3;
    [SerializeField] private float[] _zoomValues;
    public Action OnTutorial;
    public Action OnTutorialFinish;
    private Coroutine _tutoRoutine;
    BeatManager _beatManager;
    
    public int HandledTutoCharacter { get; set; }
    public int CharcterTutoAmount { get; set; }

    public bool UseTutorial => Globals.DataLoader?.IsTutoActivated ?? true;
    //public bool UseTutorial => true;
    
    private void Awake()
    {
        Globals.TutorialManager ??= this;
        OnTutorial += () => StartCoroutine(TutoRoutine());
        OnTutorialFinish += () => StartCoroutine(EndTutorialRoutine());
    }

    private IEnumerator Start()
    {
        _beatManager = Globals.BeatManager as BeatManager;
        yield return null;
        if (UseTutorial)
        {
            OnTutorialFinish += _beatManager.PlayFirstMusic;
            OnTutorial?.Invoke();
        }
    }

    IEnumerator TutoRoutine()
    {
        LightIntensityTrigger.ActivateLight(false);
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.DJ).SetShadowMaterial(true);
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.DANCEFLOOR).SetShadowMaterial(true);
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.BARMAN).SetShadowMaterial(true);
        yield return new WaitUntil(() => HandledTutoCharacter == CharcterTutoAmount || Input.GetKeyDown(KeyCode.Return));
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.DJ).SetShadowMaterial(false);
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.DANCEFLOOR).SetShadowMaterial(false);
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.BARMAN).SetShadowMaterial(false);
        Globals.CameraProfileManager.StartPulseForAll();
        for (int i = _timerValue; i > 0; --i)
        {
            Globals.CameraProfileManager.StartPulseForAllOnce(_zoomValues[i - 1], _beatManager.BeatDurationInMilliseconds/1000f);
            _timer.text = i.ToString();
            yield return new WaitForSeconds(_beatManager.BeatDurationInMilliseconds / 1000f);
        }
        _timer.text = "";
        OnTutorialFinish?.Invoke();
        _tutoRoutine = null;
    }

    private IEnumerator EndTutorialRoutine()
    {
        LightIntensityTrigger.ActivateLight(true);
        yield return null;
    }
}
