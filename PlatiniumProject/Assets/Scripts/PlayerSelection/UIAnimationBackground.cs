using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationBackground : MonoBehaviour
{
    [SerializeField] float _durationBetweenChangeImage = 0.2f;
    [SerializeField] List<Sprite> _sprites;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Image _image;
    int _indexSprite = 0;

    private void Start()
    {
        if (_spriteRenderer != null && _sprites != null && _sprites.Count > 0)
        {
            ChangeSprite();
            StartCoroutine(RoutineChangeSprite());
        }
    }

    IEnumerator RoutineChangeSprite()
    {
        while(true)
        {
            yield return new WaitForSeconds(_durationBetweenChangeImage);
            _indexSprite = (_indexSprite + 1) % _sprites.Count;
            ChangeSprite();
        }
    }

    void ChangeSprite()
    {
        if(_spriteRenderer!= null) _spriteRenderer.sprite = _sprites[_indexSprite];
        if(_image!= null) _image.sprite = _sprites[_indexSprite];
    }
}
