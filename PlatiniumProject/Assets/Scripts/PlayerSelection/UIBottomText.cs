using TMPro;
using UnityEngine;

public class UIBottomText : MonoBehaviour
{
    [SerializeField] PlayerSelectionManager _selectionManager;
    [SerializeField] GameObject _textStart;
    [SerializeField] GameObject _textSelectCharacter;

    private void Start()
    {
        _textStart.gameObject.SetActive(false);
        _textSelectCharacter.gameObject.SetActive(true);
        if (_selectionManager != null)
        {
            _selectionManager.OnAllCharacterChosen += OnAllCharacterChosen;
        }
    }

    private void OnAllCharacterChosen(bool areAllCharacterChosen)
    {
        _textStart.gameObject.SetActive(areAllCharacterChosen);
        _textSelectCharacter.gameObject.SetActive(!areAllCharacterChosen);
    }
}
