using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BouncerQTEController : MonoBehaviour, IQTEable
{
    QTEHandler _qteHandler;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] GameObject _bubbleImage;
    void Start()
    {
        TryGetComponent(out _qteHandler);
        if (_qteHandler != null)
        {
            _qteHandler.RegisterQTEable(this);
        }
        _bubbleImage.SetActive(false);
    }
    public void StartQTE()
    {
        _qteHandler.StartNewQTE();
    }

    public void OnQTEComplete()
    {
        //BUBBLE DISAPPEAR
        _bubbleImage.SetActive(false);
    }

    public void OnQTECorrectInput()
    {
        _text.text = _qteHandler.GetCurrentInputString();
    }

    public void OnQTEStarted()
    {
        //BUBBLE APPEAR
        _bubbleImage.SetActive(true);
        _text.text = _qteHandler.GetCurrentInputString();
    }

    public void OnQTEWrongInput()
    {
        _qteHandler.DeleteCurrentCoroutine();
        _bubbleImage.SetActive(false);
        _text.text = _qteHandler.GetCurrentInputString();
    }
}
