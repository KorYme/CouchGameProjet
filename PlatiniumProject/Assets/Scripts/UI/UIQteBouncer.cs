using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQteBouncer : MonoBehaviour
{
    [SerializeField] Image[] _imagesInput;
    [SerializeField] float _durationAnimation = 0.3f;
    [SerializeField] float _offset = 0.2f;
    int _currentIndex = 0;
    [Header("Current input")]
    [SerializeField] Vector3 _scaleCurrentInput = Vector3.one;
    [SerializeField] Color _colorCurrentInput = Color.white;
    [Header("Other input")]
    [SerializeField] Vector3 _scaleOtherInput = Vector3.zero;
    [SerializeField] Color _colorOtherInput = Color.white;
    Vector3[] _positions;
    void Start()
    {
        _positions = new Vector3[3];
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            _positions[i] = _imagesInput[i].transform.localPosition;
        }
        ResetDisplay();
    }

    void ResetDisplay()
    {
        _currentIndex = 0;
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            _imagesInput[i].color = i == 0 ? _colorCurrentInput : _colorOtherInput;
            _imagesInput[i].transform.localScale = i == 0 ? _scaleCurrentInput : _scaleOtherInput;
        }
        /*for (int i = 0; i < _imagesInput.Length; i++)
        {
            float offset = Mathf.Max((-1 / 2f * Mathf.Abs(1 - _currentIndex)) + 1, 0) * _offset * Mathf.Clamp(i - _currentIndex,-1,1);
            _imagesInput[i].transform.localPosition = _positions[i] + new Vector3(offset, 0f, 0f);
        }*/
    }

    public void MoveToNextInput()
    {
        if (_currentIndex < _imagesInput.Length)
        {
            _imagesInput[_currentIndex].DOColor(_colorOtherInput, _durationAnimation);
            _imagesInput[_currentIndex].transform.DOScale(_scaleOtherInput, _durationAnimation);
            _currentIndex++;
            //Widen new current input
            if (_currentIndex < _imagesInput.Length)
            {
                _imagesInput[_currentIndex].DOColor(_colorCurrentInput, _durationAnimation);
                _imagesInput[_currentIndex].transform.DOScale(_scaleCurrentInput, _durationAnimation);
                /*for (int i = 0; i < _imagesInput.Length; i++)
                {
                    float offset = Mathf.Max((-1 / 2 * Mathf.Abs(1 - _currentIndex)) + 1, 0) * _offset * Mathf.Clamp(i - _currentIndex, -1, 1);
                    _imagesInput[i].transform.DOLocalMove(_positions[i] + new Vector3(offset, 0f, 0f), _durationAnimation);
                }*/
            }
        }
    }
}
