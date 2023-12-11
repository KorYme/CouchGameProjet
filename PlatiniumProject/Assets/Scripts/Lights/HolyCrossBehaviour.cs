using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HolyCrossBehaviour : MonoBehaviour
{
    [SerializeField] Light2D _light;
    [SerializeField] CharacterStateMachine _charaSM;

    [Header("First Apperance Tween")]
    [SerializeField] AnimationCurve _fadeInCurve;
    [SerializeField] float _fadeInEndValue = 1, _fadeInDuration = 1;

    [Header("Huge Scale Tween")]
    [SerializeField] AnimationCurve _scaleCurve;
    [SerializeField] float _scaleEndValue = 10, _scaleDuration = 1;

    [Header("Other Paramaters")]
    [SerializeField] float _timeBetween;

    private void Start()
    {
        (_charaSM.ExorcizeState as CharacterStateExorcize).OnCharacterStartExorcize += () => StartCoroutine(PlayAnim());
    }

    public IEnumerator PlayAnim()
    {
        _light.enabled = true;
        float timer = _fadeInDuration == 0f ? 1f : 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime / _fadeInDuration;
            transform.localScale = Vector3.one * _fadeInCurve.Evaluate(timer) * _fadeInEndValue;
            yield return null;
        }
        transform.localScale = Vector3.one * _fadeInEndValue;
        timer = _scaleDuration == 0f ? 1f : 0f;
        yield return new WaitForSeconds(_timeBetween);
        while (timer < 1f)
        {
            timer += Time.deltaTime / _scaleDuration;
            transform.localScale = Vector3.one * _scaleCurve.Evaluate(timer) * _scaleEndValue;
            yield return null;
        }
        transform.localScale = Vector3.one * _scaleEndValue;
    }
}
