using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIQteDJ : MonoBehaviour
{
    [SerializeField] float _durationAnimation = 0.3f;
    #region Renderers
    [Header("Renderers")]
    [SerializeField] Image[] _imagesInput;
    [SerializeField] Image _imageCurrentInput;
    [SerializeField] Image _imageNextInput;
    [SerializeField] Image _imageNextInput2;
    [SerializeField] Image _imageTransition;
    #endregion

    #region InitialInfos
    Vector3[] _initialPositions;
    Vector3[] _initialScales;
    Color[] _initialColors;
    #endregion

    Sprite _newSprite;

    private void Start()
    {
        InitializeRenderers();
    }
    private void InitializeRenderers()
    {
        _initialPositions = new Vector3[4];
        _initialScales = new Vector3[4];
        InitializeInfosFromRenderer(_imageCurrentInput, 0);
        InitializeInfosFromRenderer(_imageNextInput, 1);
        InitializeInfosFromRenderer(_imageNextInput2, 2);
        InitializeInfosFromRenderer(_imageTransition, 3);
    }
    private void InitializeInfosFromRenderer(Image image,int index)
    {
        if (image != null)
        {
            _initialPositions[index] = image.transform.localPosition;
            _initialScales[index] = image.transform.localScale;
            _initialColors[index] = image.color;
        }
    }

    public void StartAnimation()
    {
        ResetInputs();

        //Current disappearing
        _imageCurrentInput.DOFade(0, _durationAnimation);
        _imageCurrentInput.transform.DOScale(0, _durationAnimation);
        //Next to current
        _imageNextInput.DOColor(Color.white, _durationAnimation);
        _imageNextInput.DOFade(1,_durationAnimation);
        _imageNextInput.transform.DOLocalMove(_initialPositions[0],_durationAnimation);
        _imageNextInput.transform.DOScale(_initialScales[0],_durationAnimation);

        /*//Next2 to next
        _imageNextInput2.DOColor(_taintNextInput, _durationAnimation);
        _imageNextInput2.DOFade(_taintNextInput.a, _durationAnimation);
        _imageNextInput2.transform.DOLocalMove(_initialPositions[1], _durationAnimation);
        _imageNextInput2.transform.DOScale(_initialScales[1], _durationAnimation);

        //Transition to next2
        _imageTransition.DOColor(_taintNextInput, _durationAnimation);
        _imageTransition.DOFade(_taintNextInput.a, _durationAnimation);
        _imageTransition.transform.DOLocalMove(_initialPositions[2], _durationAnimation);
        _imageTransition.transform.DOScale(_initialScales[2], _durationAnimation);*/

        for(int i = 0; i < _imagesInput.Length; i++)
        {
            if (i == 0)
            {
                _imagesInput[i].DOFade(0, _durationAnimation);
                _imageCurrentInput.transform.DOScale(0, _durationAnimation);
            } else
            {
                _imagesInput[i].DOColor(_initialColors[i-1], _durationAnimation);
            }
        }
    }

    private void ResetInputs()
    {
        Sprite sprite = _imageCurrentInput.sprite; //Changer par nouvel input
        _imageCurrentInput.sprite = _imageNextInput.sprite;
        _imageCurrentInput.color = _imageNextInput.color;
        _imageCurrentInput.transform.localPosition = _initialPositions[0];
        _imageCurrentInput.transform.localScale = _initialScales[0];

        _imageNextInput.sprite = _imageNextInput2.sprite;
        _imageNextInput.color = _imageNextInput2.color;
        _imageNextInput.transform.localPosition = _initialPositions[1];
        _imageNextInput.transform.localScale = _initialScales[1];

        _imageNextInput2.sprite = _imageTransition.sprite;
        _imageNextInput2.color = _imageTransition.color;
        _imageNextInput2.transform.localPosition = _initialPositions[2];
        _imageNextInput2.transform.localScale = _initialScales[2];

        _imageTransition.sprite = sprite;
        _imageTransition.color = _taintNextInput;
        _imageTransition.transform.localPosition = _initialPositions[3];
        _imageTransition.transform.localScale = _initialScales[3];
    }
}
