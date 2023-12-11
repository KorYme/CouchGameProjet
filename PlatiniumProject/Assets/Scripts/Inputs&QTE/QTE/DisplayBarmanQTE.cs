using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayBarmanQTE : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _qteDisplay;
    [SerializeField] GameObject _bubbleObject;
    [SerializeField] BarmanQTEController _qteController;

    private void Awake()
    {
        _bubbleObject.SetActive(false);
    }
    private void Start()
    {
        _qteController.OnBarmanQTEStarted += OnDJQTEStarted;
        _qteController.OnBarmanQTEEnded += OnDJQTEEnded;
        _qteController.OnBarmanQTEChanged += OnDJQTEChanged;
    }

    private void OnDestroy()
    {
        _qteController.OnBarmanQTEStarted -= OnDJQTEStarted;
        _qteController.OnBarmanQTEEnded -= OnDJQTEEnded;
        _qteController.OnBarmanQTEChanged -= OnDJQTEChanged;
    }

    private void OnDJQTEChanged(string qteString)
    {
        if (qteString == string.Empty)
        {
            OnDJQTEEnded(qteString);
        } else
        {
            _qteDisplay.text = qteString;
        }
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
