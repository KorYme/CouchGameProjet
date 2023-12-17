using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQteBarman : UIQte
{
    [SerializeField, Range(0f, 1f)] float _darkeningValue = 0f;
    [SerializeField] Image _darkeningImage;
    [SerializeField] Color _pressedInputColor = Color.black;

    private void Awake()
    {
        ResetDisplay();
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
}
