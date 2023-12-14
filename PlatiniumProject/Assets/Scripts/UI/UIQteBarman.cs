using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQteBarman : UIQte
{
    [SerializeField, Range(0f, 1f)] float _darkeningValue = 0f;
    [SerializeField] Image _darkeningImage;

    private void OnValidate()
    {
        UpdateDarkeningValue();
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
