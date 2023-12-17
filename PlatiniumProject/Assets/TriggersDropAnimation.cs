using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggersDropAnimation : MonoBehaviour
{
    [SerializeField] Sprite _released, _triggered;
    [SerializeField] Image _image;
    bool _isTriggered;

    private void Reset()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        _isTriggered = false;
        Globals.BeatManager.OnBeatEvent.AddListener(ChangeSprite);
    }

    void ChangeSprite()
    {
        _isTriggered = !_isTriggered;
        _image.sprite = _isTriggered ? _triggered : _released;
    }
}
