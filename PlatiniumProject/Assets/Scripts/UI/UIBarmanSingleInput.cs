using UnityEngine;
using UnityEngine.UI;

public class UIBarmanSingleInput : MonoBehaviour
{
    [SerializeField] Image _imageInput;
    [SerializeField] Image _imageInputDarkening;
    public float FillAmount
    {
        get {
            return _imageInputDarkening == null ? 0f : _imageInputDarkening.fillAmount;
        }
        set
        {
            if (_imageInputDarkening != null)
                _imageInputDarkening.fillAmount = value;
        }
    }

    private void OnValidate()
    {
        if (_imageInputDarkening != null)
            _imageInputDarkening.sprite = _imageInput.sprite;
    }

    void Start()
    {
        _imageInputDarkening.sprite = _imageInput.sprite;
        _imageInputDarkening.fillAmount = 0f;
    }

    public void ChangeSprite(Sprite newImage)
    {
        _imageInput.sprite = newImage;
        _imageInputDarkening.sprite = newImage;
        _imageInputDarkening.fillAmount = 0f;
    }
}
