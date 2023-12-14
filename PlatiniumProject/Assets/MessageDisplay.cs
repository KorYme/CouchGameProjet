using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MessageDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private List<GameObject> _gameObjects;

    [SerializeField] string _pressingMessage, _releasingMessage;

    [SerializeField] private float _offSetValue;
    [SerializeField] private float _delayBetweenTween;

    private void Reset()
    {
        _gameObjects.Clear();
        foreach (Transform childTransform in transform)
        {
            _gameObjects.Add(childTransform.gameObject);
        }
    }

    private void Start()
    {
        Globals.DropManager.OnBeginBuildUp += () => DisplayMessage(_pressingMessage);
        Globals.DropManager.OnDropLaunched += () => DisplayMessage(_releasingMessage);
    }

    private void OnDisable()
    {
        Globals.DropManager.OnBeginBuildUp -= () => DisplayMessage(_pressingMessage);
        Globals.DropManager.OnDropLaunched -= () => DisplayMessage(_releasingMessage);
    }

    public void DisplayMessage(string message)
    {
        _text.text = message.ToUpper();
        _gameObjects.ForEach(gameObject => gameObject.SetActive(true));
        DOTweenTMPAnimator animator = new DOTweenTMPAnimator(_text);
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < animator.textInfo.characterCount; ++i)
        {
            animator.DOColorChar(i, Vector4.zero, 0f);
            animator.DOOffsetChar(i, Vector3.one * -_offSetValue, 0f);
        }

        for (int i = animator.textInfo.characterCount - 1 ; i >= 0; --i)
        {
            sequence.Join(animator.DOOffsetChar(i, Vector3.one * _offSetValue, .25f)).SetEase(Ease.InCubic).SetDelay(_delayBetweenTween);
            sequence.Join(animator.DOColorChar(i, Color.red, .1f)).SetEase(Ease.InCubic).SetDelay(_delayBetweenTween);
        }

        sequence.Append(animator.DOOffsetChar(animator.textInfo.characterCount - 1, Vector3.one * _offSetValue, 0f).SetDelay(.25f));
        sequence.Append(animator.DOColorChar(animator.textInfo.characterCount - 1, Color.red, .1f));
        sequence.Append(animator.DOScaleChar(animator.textInfo.characterCount - 1, Vector3.one * 2, .5f)).SetLoops(2, LoopType.Yoyo);
        sequence.Join(animator.DOPunchCharRotation(animator.textInfo.characterCount - 1, new Vector3(0,0,75), .5f, 10, 50f).SetEase(Ease.InOutFlash)).SetLoops(2, LoopType.Yoyo);
        sequence.onComplete += () => _gameObjects.ForEach(gameObject => gameObject.SetActive(false));
    }
}
