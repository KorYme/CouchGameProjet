using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQteBarman : UIQte
{
    [SerializeField, Range(0f, 1f)] float _darkeningValue = 0f;
    [SerializeField] Image _darkeningImage;
    [SerializeField] Color _pressedInputColor = Color.black;
    Vector3 _initialSize;

    private void Awake()
    {
        ResetDisplay();
        if (_imagesInput == null || _imagesInput.Length > 0)
            _initialSize = _imagesInput[0].transform.localScale;
    }

    public void ChangeDarkeningValue(float value)
    {
        _darkeningValue = value;
        UpdateDarkeningValue();
    }

    public void ChangeColors(bool[] colors)
    {
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            if (_imagesInput[i].sprite != null && colors[i])
            {
                _imagesInput[i].color = _pressedInputColor;
            } else if (_imagesInput[i].sprite != null)
            {
                _imagesInput[i].color = Color.white;
            } else {
                _imagesInput[i].color = Color.clear;
            }
        }
    }
    void UpdateDarkeningValue()
    {
        if (_darkeningImage != null)
        {
            _darkeningImage.fillAmount = _darkeningValue;
        }
    }

    protected override void ResetDisplay()
    {
        _darkeningValue = 0f;
        UpdateDarkeningValue();
    }

    protected override void ModifyDisplay()
    {
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            if (_imagesInput[i].sprite != null)
            {
                _imagesInput[i].color = Color.white;
                _imagesInput[i].transform.localScale = _initialSize;
            }
            else
            {
                _imagesInput[i].color = Color.clear;
                _imagesInput[i].transform.localScale = Vector3.zero;
            }
        }
    }
}
