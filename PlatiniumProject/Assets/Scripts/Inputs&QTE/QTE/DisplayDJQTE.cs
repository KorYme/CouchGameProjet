using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayDJQTE : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _qteDisplay;
    [SerializeField] GameObject _bubbleObject;
    [SerializeField] DJQTEController _qteController;

    private void Awake()
    {
        _bubbleObject.SetActive(false);
    }
    private void Start()
    {
        _qteController.OnDJQTEStarted += OnDJQTEStarted;
        _qteController.OnDJQTEEnded += OnDJQTEEnded;
        _qteController.OnDJQTEChanged += OnDJQTEChanged;
    }

    private void OnDestroy()
    {
        _qteController.OnDJQTEStarted -= OnDJQTEStarted;
        _qteController.OnDJQTEEnded -= OnDJQTEEnded;
        _qteController.OnDJQTEChanged -= OnDJQTEChanged;
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
