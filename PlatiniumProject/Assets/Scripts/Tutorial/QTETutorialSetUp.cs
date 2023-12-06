using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTETutorialSetUp : MonoBehaviour
{
    [SerializeField] QTEHandler _handler;
    void Start()
    {
        Globals.TutorialManager.OnTutorial += ChangeQTEForTutorial;
        Globals.TutorialManager.OnTutorialFinish += ChangeQTEForTutorialEnd;
    }

    private void OnDestroy()
    {
        Globals.TutorialManager.OnTutorial -= ChangeQTEForTutorial;
        Globals.TutorialManager.OnTutorialFinish -= ChangeQTEForTutorialEnd;
    }
    private void ChangeQTEForTutorial()
    {
        _handler.WaitForCorrectInput = true;
    }

    private void ChangeQTEForTutorialEnd()
    {
        _handler.WaitForCorrectInput = false;
    }
}
