using Rewired;
using Rewired.Integration.UnityUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBasicFunctions : MonoBehaviour
{
    [SerializeField] RewiredStandaloneInputModule _rewiredUIModule;
    private void Start()
    {
        _rewiredUIModule.RewiredInputManager = FindObjectOfType<InputManager>();
    }

    public void Quit() => Application.Quit();
}
