using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionHandler : MonoBehaviour
{
    [SerializeField] PlayerRole _role;
    [SerializeField] PlayerSelectionManager _selectionManager;
    [SerializeField] float _durationScale = 0.2f;
    [SerializeField] float _forceScale = 1f;
    [SerializeField] Ease _curveScale = Ease.InOutSine;

    public PlayerRole Role { get => _role;}

    private void Start()
    {
        if (_selectionManager != null)
        {
            _selectionManager.OnPlayerChooseCharacter += OnPlayerChooseCharacter;
        }
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
            transform.DOKill();
            transform.DOScale(Vector3.one + Vector3.one * _forceScale, _durationScale).SetEase(_curveScale).SetLoops(2,LoopType.Yoyo);
        }
    }
}
