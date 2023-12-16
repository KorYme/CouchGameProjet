using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

public class DisplayBarmanQTE : MonoBehaviour
{
    [SerializeField] UIQteBarman _qteDisplay;
    [SerializeField] GameObject _bubbleObject;
    [SerializeField] BarmanQTEController _qteController;

    private void Start()
    {
        _bubbleObject.SetActive(false);
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

    private void OnDJQTEChanged(Sprite[] sprites, bool[] colors,float value)
    {
        if (sprites == null || sprites.Length == 0)
        {
            OnDJQTEEnded(sprites,colors);
        } else
        {
            _qteDisplay.ChangeDarkeningValue(value);
            _qteDisplay.ChangeColors(colors);
            //_qteDisplay.text = qteString;
        }
    }

    private void OnDJQTEEnded(Sprite[] sprites, bool[] colors)
    {
        //_qteDisplay.text = qteString;
        _bubbleObject.SetActive(false);
    }

    private void OnDJQTEStarted(Sprite[] sprites, bool[] inputs)
    {
        _bubbleObject.SetActive(true);
        _qteDisplay.ChangeSprites(sprites);
        _qteDisplay.ChangeColors(inputs);
    }
}
