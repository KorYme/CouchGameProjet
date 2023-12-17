using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIQteDJ : UIQte
{
    struct InputSprite
    {
        public Vector3 PositionSprite;
        public Vector3 ScaleSprite;
        public Color ColorSprite;
    }
    [SerializeField] float _durationAnimation = 0.3f;
    #region Renderers
    [Header("Renderers")]
    #endregion
    InputSprite[] _initialSpriteInfos;

    public Sprite NextSprite { get; set; } = null;
    private bool _needRestart = false;

    private void Awake()
    {
        InitializeRenderers();
    }
    private void InitializeRenderers()
    {
        if (_imagesInput.Length > 0)
        {
            _initialSpriteInfos = new InputSprite[_imagesInput.Length];
            InitializeInfosFromRenderers();
        }
    }
    private void InitializeInfosFromRenderers()
    {
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            _initialSpriteInfos[i].PositionSprite = _imagesInput[i].transform.localPosition;
            _initialSpriteInfos[i].ScaleSprite = _imagesInput[i].transform.localScale;
            
            _initialSpriteInfos[i].ColorSprite = _imagesInput[i].color;
        }
    }

    private void SnapImagesToEndAnimation()
    {
        for (int i = _imagesInput.Length - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                _imagesInput[i].color = new Color(1, 1, 1, 0);
                _imagesInput[i].transform.localScale = Vector3.zero;
            }
            else
            {
                if (_imagesInput[i].sprite != null)
                {
                    _imagesInput[i].sprite = _imagesInput[i - 1].sprite;
                    _imagesInput[i].color = _initialSpriteInfos[i - 1].ColorSprite;
                }
                _imagesInput[i].transform.localPosition = _initialSpriteInfos[i - 1].PositionSprite;
                _imagesInput[i].transform.localScale = _initialSpriteInfos[i - 1].ScaleSprite;
            }
        }
    }
    public void StartAnimation()
    {
        /*ResetInputs();
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            if (i == 0)
            {
                _imagesInput[i].DOFade(0, _durationAnimation);

                _imagesInput[i].transform.DOScale(0, _durationAnimation);
            }
            else
            {
                if (_imagesInput[i].sprite != null)
                {
                    _imagesInput[i].DOColor(_initialSpriteInfos[i - 1].ColorSprite, _durationAnimation);
                    _imagesInput[i].DOFade(_initialSpriteInfos[i - 1].ColorSprite.a, _durationAnimation);

                }
                _imagesInput[i].transform.DOLocalMove(_initialSpriteInfos[i - 1].PositionSprite, _durationAnimation);
                _imagesInput[i].transform.DOScale(_initialSpriteInfos[i - 1].ScaleSprite, _durationAnimation);
            }
        }*/
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            if (i < _imagesInput.Length - 1)
            {
                _imagesInput[i].sprite = _imagesInput[i + 1].sprite;
            } else
            {
                _imagesInput[i].sprite = NextSprite;
            }
        }
    }
    private void ResetInputs()
    {
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            if (i != _imagesInput.Length - 1 && _imagesInput[i + 1].sprite != null)
            {
                _imagesInput[i].color = _initialSpriteInfos[i].ColorSprite;
                if (_needRestart)
                {
                    _imagesInput[i].sprite = _imagesInput[i + 1].sprite;
                }
            }
            else
            {
                _imagesInput[i].sprite = NextSprite;
                _imagesInput[i].color = Color.clear;
            }
            _imagesInput[i].transform.localPosition = _initialSpriteInfos[i].PositionSprite;
            _imagesInput[i].transform.localScale = _initialSpriteInfos[i].ScaleSprite;
        }
        if (!_needRestart)
        {
            _needRestart = true;
        }
    }

    protected override void ResetDisplay()
    {
        _needRestart = false;
        //SnapImagesToEndAnimation();
    }
    protected override void ModifyDisplay()
    {
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            _imagesInput[i].color = _imagesInput[i].sprite == null ? Color.clear: _initialSpriteInfos[i].ColorSprite;
        }
    }
}