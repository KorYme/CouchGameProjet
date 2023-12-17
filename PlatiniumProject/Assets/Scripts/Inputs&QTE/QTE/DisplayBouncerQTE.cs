using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayBouncerQTE : MonoBehaviour
{
    [SerializeField] GameObject _qteBubbleObject;
    [SerializeField] GameObject _checkingBubbleObject;
    [SerializeField] BouncerQTEController _qteController;
    [SerializeField] UIQteBouncer _qteDisplay;

    private void Start()
    {
        _qteBubbleObject.SetActive(false);
        _checkingBubbleObject.SetActive(false);
        _qteController.OnBouncerCheckingStarted += OnBouncerCheckingStarted;
        _qteController.OnBouncerQTEStarted += OnDJQTEStarted;
        _qteController.OnBouncerQTEEnded += OnDJQTEEnded;
        _qteController.OnBouncerQTEChanged += OnDJQTEChanged;
    }

    private void OnDestroy()
    {
        _qteController.OnBouncerCheckingStarted -= OnBouncerCheckingStarted;
        _qteController.OnBouncerQTEStarted -= OnDJQTEStarted;
        _qteController.OnBouncerQTEEnded -= OnDJQTEEnded;
        _qteController.OnBouncerQTEChanged -= OnDJQTEChanged;
    }
    private void OnDJQTEChanged(Sprite[] sprites)
    {
        _qteDisplay.MoveToNextInput();
    }

    private void OnDJQTEEnded(Sprite[] sprites)
    {
        _qteBubbleObject.SetActive(false);
        _checkingBubbleObject.SetActive(false);
    }

    private void OnDJQTEStarted(Sprite[] sprites)
    {
        _qteBubbleObject.SetActive(true);
        _checkingBubbleObject.SetActive(false);
        _qteDisplay.ChangeSprites(sprites);
    }
    private void OnBouncerCheckingStarted()
    {
        _qteBubbleObject.SetActive(false);
        _checkingBubbleObject.SetActive(true);
    }
}
