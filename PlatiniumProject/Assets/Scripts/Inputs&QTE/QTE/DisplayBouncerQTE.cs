using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayBouncerQTE : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _qteDisplay;
    [SerializeField] GameObject _bubbleObject;
    [SerializeField] BouncerQTEController _qteController;

    private void Awake()
    {
        _bubbleObject.SetActive(false);
    }
    private void Start()
    {
        _qteController.OnBouncerQTEStarted += OnDJQTEStarted;
        _qteController.OnBouncerQTEEnded += OnDJQTEEnded;
        _qteController.OnBouncerQTEChanged += OnDJQTEChanged;
    }

    private void OnDJQTEChanged(string qteString)
    {
        _qteDisplay.text = qteString;
    }

    private void OnDJQTEEnded(string qteString)
    {
        _qteDisplay.text = qteString;
        _bubbleObject.SetActive(false);
    }

    private void OnDJQTEStarted(string qteString)
    {
        _bubbleObject.SetActive(true);
        _qteDisplay.text = qteString;
    }
}
