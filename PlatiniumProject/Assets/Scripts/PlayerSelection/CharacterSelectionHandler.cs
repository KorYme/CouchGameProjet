using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionHandler : MonoBehaviour
{
    [SerializeField] PlayerRole _role;
    [SerializeField] PlayerSelectionManager _selectionManager;
    [Space(10)]
    [Header("Scale")]
    [SerializeField] float _durationScale = 0.2f;
    [SerializeField] float _forceScale = 1f;
    [SerializeField] Ease _curveScale = Ease.InOutSine;
    Vector3 _normalScale;
    [Space(10)]
    [Header("Sprites")]
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Sprite _unselectedSprite;
    [SerializeField] Sprite _selectedSprite;

    public PlayerRole Role { get => _role;}

    private void Start()
    {
        _normalScale = transform.localScale;
        if (_selectionManager != null)
        {
            _selectionManager.OnPlayerChooseCharacter += OnPlayerChooseCharacter;
            _selectionManager.OnPlayerUnchooseCharacter += OnPlayerUnchooseCharacter; ;
        }
        if (_spriteRenderer != null)
            _spriteRenderer.sprite = _unselectedSprite;
    }

    private void OnDestroy()
    {
        if (_selectionManager != null)
        {
            _selectionManager.OnPlayerChooseCharacter -= OnPlayerChooseCharacter;
        }
    }
    private void OnPlayerChooseCharacter(int indexPlayer, int indexCharacter,PlayerRole role)
    {
        if (_role == role)
        {
            if (_spriteRenderer != null)
                _spriteRenderer.sprite = _selectedSprite;
            transform.DOKill();
            transform.localScale = _normalScale;
            transform.DOScale(_normalScale * (1 + _forceScale), _durationScale).SetEase(_curveScale).SetLoops(2,LoopType.Yoyo);
        }
    }

    private void OnPlayerUnchooseCharacter(int indexPlayer, int indexCharacter, PlayerRole role)
    {
        if (_role == role) //Ce joueur est déselectionné
        {
            if (_spriteRenderer != null)
                _spriteRenderer.sprite = _unselectedSprite;
        }
    }
}
