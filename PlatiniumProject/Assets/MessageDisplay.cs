using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MessageDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    [SerializeField] private float _offSetValue;
    [SerializeField] private float _delayBetweenTween;
    
    private void Start()
    {
        Globals.DropManager.OnBeginBuildUp += DisplayMessage;
    }

    private void OnDisable()
    {
        Globals.DropManager.OnBeginBuildUp -= DisplayMessage;
    }

    public void DisplayMessage()
    {
        text.gameObject.SetActive(true);
        DOTweenTMPAnimator animator = new DOTweenTMPAnimator(text);
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
    }
}
