using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayQTE : MonoBehaviour,IQTEable
{
    [SerializeField] TextMeshPro _qteDisplay;

    public void OnQTEComplete()
    {
        throw new System.NotImplementedException();
    }

    public void OnQTECorrectInput()
    {
        throw new System.NotImplementedException();
    }

    public void OnQTEStarted()
    {
    }

    public void OnQTEWrongInput()
    {
        throw new System.NotImplementedException();
    }
}
