using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DarkenSpriteCharacter : MonoBehaviour
{
    [SerializeField] PlayerSelectionManager _selectionManager;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Color _unlitSpriteColor = Color.black;
    [SerializeField] Color _litSpriteColor = Color.white;
    int _nbPlayersOnCharacter = 0;
    [SerializeField] int _indexCharacter = 0;
    [SerializeField] float _colorTransitionDuration = 0.2f;
    [SerializeField] UnityEvent _onPlayerMove;
    Color _targetColor;
    Coroutine _routineChangeLight;

    IEnumerator Start()
    {
        if (_selectionManager != null)
        {
            _selectionManager.OnPlayerMove += OnPlayerMove;
            _selectionManager.OnPlayerJoined += OnPlayerJoined;
            LightSprite(false);
            yield return new WaitUntil(() => _selectionManager.IsSetUp);
            _nbPlayersOnCharacter = _selectionManager.NbPlayersHoverOnCharacter(_indexCharacter);
            UpdateSmoothlyLuminosity();
        }
    }

    private void OnDestroy()
    {
        if (_selectionManager != null)
        {
            _selectionManager.OnPlayerMove -= OnPlayerMove;
            _selectionManager.OnPlayerJoined -= OnPlayerJoined;
        }
    }

    private void OnPlayerJoined(int arg1, int arg2)
    {
        if(arg2 == _indexCharacter)
        {
            _nbPlayersOnCharacter++;
        }
        UpdateSmoothlyLuminosity();
    }

    private void OnPlayerMove(int indexPlayer, int newCharacter,int lastCharacter)
    {
        _onPlayerMove?.Invoke();
        if (newCharacter == _indexCharacter)
        {
            _nbPlayersOnCharacter++;
        }
        if (lastCharacter == _indexCharacter)
        {
            _nbPlayersOnCharacter--;
        }
        UpdateSmoothlyLuminosity();
    }

    private void LightSprite(bool enlighten)
    {
        _spriteRenderer.color = enlighten ? _litSpriteColor : _unlitSpriteColor;
    }

    private void UpdateSmoothlyLuminosity()
    {
        _targetColor = _nbPlayersOnCharacter > 0 ? _litSpriteColor : _unlitSpriteColor;
        if (_targetColor != _spriteRenderer.color && _routineChangeLight == null)
            _routineChangeLight = StartCoroutine(RoutineUpdateSmoothlyLuminosity());
    }

    private IEnumerator RoutineUpdateSmoothlyLuminosity()
    {
        while (_targetColor != _spriteRenderer.color)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _targetColor, Time.deltaTime / _colorTransitionDuration);
            yield return null;
        }
        _routineChangeLight = null;
    }
}
