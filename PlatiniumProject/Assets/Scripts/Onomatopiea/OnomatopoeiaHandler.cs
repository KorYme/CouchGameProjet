using System;
using System.Collections;
using System.Collections.Generic;
using CharTween;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class OnomatopoeiaHandler : MonoBehaviour
{

    [SerializeField] private AnimationCurve _positionCurve;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _delay;
    [SerializeField] private Vector2 _disatance;

    private List<Sequence> _sequences = new List<Sequence>();
    
    public void TestRoutine()
    {
        DOTweenTMPAnimator animator = new DOTweenTMPAnimator(_text);
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < animator.textInfo.characterCount; ++i) {
            if (!animator.textInfo.characterInfo[i].isVisible) continue;
            sequence.Join(animator.DOColorChar(i, Vector4.zero, 0));
        }
        
        for (int i = 0; i < animator.textInfo.characterCount; ++i) {
            if (!animator.textInfo.characterInfo[i].isVisible) continue;
            
            Vector3 currCharOffset = animator.GetCharOffset(i);
            sequence.Append(animator.DOColorChar(i, Color.white, 0));
            sequence.Join(animator.DOOffsetChar(i, new Vector2(0 , _positionCurve.Evaluate((float)i / animator.textInfo.characterCount))* 100, .1f));
            //sequence.Append(animator.DOOffsetChar(i, currCharOffset + new Vector3(0, 30, 0), 0));
            //sequence.Join(animator.DOOffsetChar(i, Vector2.one * 100, 0f));
        }
    }

    public void TestRoutine2()
    {
        StartCoroutine(TestRoutine3());
    }
    
    public IEnumerator TestRoutine3()
    {
        Sequence sequence = DOTween.Sequence();
        CharTweener charTweener = _text.GetCharTweener();
        
        for (int i = 0; i < charTweener.CharacterCount; ++i) {
            
            sequence.Join(charTweener.DOMove(i, charTweener.transform.position , 0));
            sequence.Join(charTweener.DOColor(i, Vector4.zero , 0));
        }
        
        for (int i = charTweener.CharacterCount; i >= 0; --i)
        {
            sequence.Join(charTweener.DOColor(i, Color.white, 0));
            AddSequence(charTweener, i);
            yield return new WaitForSeconds(_delay);
        }
    }

    private void AddSequence(CharTweener charTweener, int i)
    {
        Sequence s = DOTween.Sequence();
        _sequences.Add(s);
        for (int j = 0; j <= charTweener.CharacterCount; ++j)
        {
            s.Append(charTweener.DOMove(i,
                (Vector2)charTweener.transform.position + new Vector2((float)j / charTweener.CharacterCount,
                    _positionCurve.Evaluate((float)j / charTweener.CharacterCount)) * _disatance, .1f));
        }

        s.OnComplete(() =>
        {
            charTweener.DOColor(i, Vector4.zero, 0);
            _sequences.Remove(s);
        });
    }
}
