using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private bool _useTutorial;
    public Action OnTutorial;
    public Action OnTutorialFinish;
    private Coroutine _tutoRoutine;
    
    public int HandledTutoCharacter { get; set; }
    public int CharcterTutoAmount { get; set; }

    public bool UseTutorial => _useTutorial;
    
    private void Awake()
    {
        Globals.TutorialManager ??= this;
        OnTutorial += () => StartCoroutine(TutoRoutine());
        OnTutorialFinish += () => StartCoroutine(EndTutorialRoutine());
    }

    private IEnumerator Start()
    {
        yield return null;
        if (_useTutorial)
        {
            OnTutorial?.Invoke();
        }
    }

    IEnumerator TutoRoutine()
    {
        LightIntensityTrigger.ActivateLight(false);
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.DJ).SetShadowMaterial(true);
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.DANCEFLOOR).SetShadowMaterial(true);
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.BARMAN).SetShadowMaterial(true);
        yield return new WaitUntil(() => HandledTutoCharacter == CharcterTutoAmount);
        Debug.Log("SSSSSSSSSSSSSSSSSSSSSS");
        OnTutorialFinish?.Invoke();
        _tutoRoutine = null;
    }

    private IEnumerator EndTutorialRoutine()
    {
        LightIntensityTrigger.ActivateLight(true);
        yield return null;
    }
}
