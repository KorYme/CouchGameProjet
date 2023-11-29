using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerConnected : MonoBehaviour
{
    [SerializeField] int _indexPlayer;
    [SerializeField] TextMeshProUGUI _textDisplay;
    PlayerSelectionManager _selectionManager;

    private void Start()
    {
        _selectionManager = FindObjectOfType<PlayerSelectionManager>();
        if( _selectionManager != null)
        {
            _selectionManager.OnPlayerJoined += ChangeDisplay;
        }
    }

    void ChangeDisplay(int indexPlayer,int indexCharacter)
    {
        if (indexPlayer == _indexPlayer )
        {
            _textDisplay.text = "Player " + (_indexPlayer + 1) + " connected";
        }
    }

    private void OnDestroy()
    {
        if (_selectionManager != null)
        {
            _selectionManager.OnPlayerJoined -= ChangeDisplay;
        }
    }
}
