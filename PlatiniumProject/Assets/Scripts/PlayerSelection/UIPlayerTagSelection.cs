using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerTagSelection : MonoBehaviour
{
    [SerializeField] private int _indexCharacter = 0;
    [SerializeField] ListPlayerTag _tagList;
    [SerializeField] Image _image;
    [SerializeField] float _timeToAppear = 0.5f;
    [SerializeField]PlayerSelectionManager _selectionManager;

    private void OnValidate()
    {
        _indexCharacter = Mathf.Max(0, _indexCharacter);
    }

    void Start()
    {
        _selectionManager = FindObjectOfType<PlayerSelectionManager>();
        _selectionManager.OnPlayerChooseCharacter += OnPlayerChooseCharacter;
        _selectionManager.OnPlayerUnchooseCharacter += OnPlayerUnchooseCharacter;
        if (_image != null)
        {
            _image.transform.localScale = Vector3.zero;
            _image.sprite = null;
        }
    }

    private void OnDestroy()
    {
        _selectionManager.OnPlayerChooseCharacter -= OnPlayerChooseCharacter;
        _selectionManager.OnPlayerUnchooseCharacter -= OnPlayerUnchooseCharacter;
    }

    private void OnPlayerChooseCharacter(int indexPlayer, int indexTagCharacter, PlayerRole role)
    {
        DisplayPlayer(indexPlayer,indexTagCharacter);
    }

    private void OnPlayerUnchooseCharacter(int indexPlayer, int indexTagCharacter, PlayerRole role)
    {
        HidePlayer(indexPlayer,indexTagCharacter);
    }

    private void DisplayPlayer(int indexPlayer, int indexTagCharacter)
    {
        if (_indexCharacter == indexTagCharacter)
        {
            if (indexPlayer >= 0 && indexPlayer < _tagList.PlayerTagSprites.Count)
            {
                _image.transform.DOKill();
                _image.sprite = _tagList.PlayerTagSprites[indexPlayer];
                _image.transform.DOScale(1, _timeToAppear).SetEase(Ease.OutBack);
            } else
            {
                Debug.LogWarning("Can't display player tag, sprite not found");
            }
        }
    }

    private void HidePlayer(int indexPlayer, int indexTagCharacter)
    {
        if (_indexCharacter == indexTagCharacter)
        {
            _image.transform.DOKill();
            _image.transform.DOScale(0, _timeToAppear).SetEase(Ease.InBack).OnComplete(() => _image.sprite = null);
        }
    }
}
