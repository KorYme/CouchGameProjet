using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayDJQTE : MonoBehaviour
{
    [SerializeField] GameObject _bubbleObject;
    [SerializeField] DJQTEController _qteController;
    [SerializeField] UIQteDJ _qteDisplay;

    private void Awake()
    {
        
        _qteController.OnDJQTEStarted += OnDJQTEStarted;
        _qteController.OnDJQTEEnded += OnDJQTEEnded;
        _qteController.OnDJQTEChanged += OnDJQTEChanged;
    }
    private void Start()
    {
        _bubbleObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _qteController.OnDJQTEStarted -= OnDJQTEStarted;
        _qteController.OnDJQTEEnded -= OnDJQTEEnded;
        _qteController.OnDJQTEChanged -= OnDJQTEChanged;
    }
    private void OnDJQTEChanged(Sprite[] sprites)
    {
        //_qteDisplay.text = qteString;
        Sprite sprite = _qteController.IndexCurrentInput + 2 < sprites.Length ? sprites[_qteController.IndexCurrentInput + 2] : null;
        //Debug.Log("INDEX " + _qteController.IndexCurrentInput + " "+sprite);
        _qteDisplay.NextSprite = sprite;
        _qteDisplay.StartAnimation();
    }

    private void OnDJQTEEnded(Sprite[] sprites)
    {
        //_qteDisplay.text = qteString;
        _bubbleObject.SetActive(false);
    }

    private void OnDJQTEStarted(Sprite[] sprites)
    {
        if (sprites != null && sprites.Length > 0)
        {
            _bubbleObject.SetActive(true);
            Sprite[] spritesFistInputs = new Sprite[4];
            for (int i = 0; i < spritesFistInputs.Length; i++)
            {
                spritesFistInputs[i] = _qteController.IndexCurrentInput + i < sprites.Length ? sprites[_qteController.IndexCurrentInput + i]:null;
            }
            _qteDisplay.ChangeSprites(spritesFistInputs);
            _qteDisplay.NextSprite = spritesFistInputs[3];
            //_qteDisplay.text = qteString;
        }
    }
}
