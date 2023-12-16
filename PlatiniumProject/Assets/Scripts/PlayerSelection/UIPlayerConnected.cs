using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerConnected : MonoBehaviour
{
    [SerializeField] int _indexPlayer;
    [SerializeField] TextMeshProUGUI _textDisplay;
    [SerializeField] PlayerSelectionManager _selectionManager;

    private void Start()
    {
        if (PlayerInputsAssigner.GetRewiredPlayerById(_indexPlayer) != null)
        {
            ModifyText();
        }
        if (_selectionManager != null)
        {
            _selectionManager.OnPlayerJoined += ChangeDisplay;
        }
    }

    void ChangeDisplay(int indexPlayer,int indexCharacter)
    {
        if (indexPlayer == _indexPlayer )
        {
            ModifyText();
        }
    }

    void ModifyText()
    {
        _textDisplay.text = "PLAYER " + (_indexPlayer + 1) + " CONNECTED";
    }

    private void OnDestroy()
    {
        if (_selectionManager != null)
        {
            _selectionManager.OnPlayerJoined -= ChangeDisplay;
        }
    }
}
