using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissedInputController : MonoBehaviour,IMissedInputListener
{
    [SerializeField] QTEHandler _handler;
    bool _isdelayFinished = true;
    [SerializeField] float _delayBetweenFeedbacks = 0.1f;
    [Header("Rumbles")]
    [SerializeField] RumbleController _rumbleController;
    [SerializeField] string _rumbleName = "MissedInput";
    private void Start()
    {
        _handler.RegisterListener(this);
    }
    private void OnDestroy()
    {
        _handler.UnregisterListener(this);
    }
    public void OnQTEMissedInput()
    {
        if (_isdelayFinished)
            StartCoroutine(CoroutinewaitForDelay());
    }
    IEnumerator CoroutinewaitForDelay()
    {
        _isdelayFinished = false;
        StartFeedbacks();
        yield return new WaitForSeconds(_delayBetweenFeedbacks);
        _isdelayFinished = true;
    }

    private void StartFeedbacks()
    {
        if (_rumbleController != null)
        {
            Debug.Log("RUMBLE");
            _rumbleController.PlayRumbles(_rumbleName);
        }
    }
}
