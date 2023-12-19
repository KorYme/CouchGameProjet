using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private List<GameObject> _gameObjects;

    [SerializeField] string _pressingMessage, _releasingMessage;

    [SerializeField] private float _offSetValue;
    [SerializeField] private float _delayBetweenTween;

    [SerializeField] private Image _messageBackGround;

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
        Sequence BgSequence = DOTween.Sequence();
        
        BgSequence.Append(_messageBackGround.rectTransform.DOScale(.65f, .05f).SetEase(Ease.InOutFlash));
        BgSequence.Append(_messageBackGround.rectTransform.DOScale(.65f * 1.3f,.15f).SetEase(Ease.InOutBounce).SetLoops(2, LoopType.Yoyo));
        _text.text = message.ToUpper();
        _gameObjects.ForEach(gameObject => gameObject.SetActive(true));
        DOTweenTMPAnimator animator = new DOTweenTMPAnimator(_text);
        Sequence sequence = DOTween.Sequence();
        sequence.OnStart(() => _messageBackGround.gameObject.SetActive(true));
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
        sequence.Append(animator.DOScaleChar(animator.textInfo.characterCount - 1, Vector3.one * 2, .5f));
        sequence.Join(animator.DOScaleChar(animator.textInfo.characterCount - 1, Vector3.one *2.5f, .5f).SetEase(Ease.InOutFlash).SetLoops(2, LoopType.Yoyo)).onComplete += () =>
        {
            _gameObjects.ForEach(gameObject => gameObject.SetActive(false));
            _messageBackGround.rectTransform.localScale = Vector3.one;
            _messageBackGround.gameObject.SetActive(false);
        };;
    }

    #if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
            DisplayMessage(_pressingMessage);
    }
    #endif
}
