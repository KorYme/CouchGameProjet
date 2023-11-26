using TMPro;
using UnityEngine;

public class UIBottomText : MonoBehaviour
{
    [SerializeField] PlayerSelectionManager _selectionManager;
    [SerializeField] TextMeshProUGUI _textStart;
    [SerializeField] TextMeshProUGUI _textSelectCharacter;

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
