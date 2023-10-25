using System;
using System.Collections;
using UnityEngine;

public class QTEHandler : MonoBehaviour
{
    [SerializeField] PlayerRole _role;
    [SerializeField] PlayerInputController _playerController;
    int _indexInSequence = 0;
    QTESequence _currentQTESequence;
    BeatManager _beatManager;

    public static event Action OnInputCorrect;
    public static event Action OnSequenceComplete;

    private void Start()
    {
        ChangeQTE();
        _beatManager = FindObjectOfType<BeatManager>();
    }
    public void ChangeQTE()
    {
        _currentQTESequence = QTELoader.Instance.GetRandomQTE(_role);
        _indexInSequence = 0;
        Debug.Log($"SEQUENCE CHOSEN{_currentQTESequence.Difficulty} {_currentQTESequence.SequenceType} {_currentQTESequence.Index}");
        switch (_currentQTESequence.SequenceType)
        {
            case InputsSequence.SEQUENCE:
                StartCoroutine(StartRoutineSequence());
                break;
            case InputsSequence.SIMULTANEOUS:
                //TO DO
                break;
        }
    }

    bool CheckInput(UnitInput input)
    {
        bool isInputCorrect = false;
        switch (input.Status)
        {
            case InputStatus.PRESS:
                isInputCorrect = _playerController.GetInput(input);
                break;
            case InputStatus.HOLD:
                isInputCorrect = _playerController.GetInputHold(input);
                break;
        }
        return isInputCorrect;
    }

    IEnumerator StartRoutineSequence()
    {
        UnitInput input = _currentQTESequence.ListSubHandlers[_indexInSequence];
        while (_indexInSequence < _currentQTESequence.ListSubHandlers.Count)
        {
            if (_beatManager.IsInsideBeat)
            {
                if (CheckInput(input))
                {
                    if (_indexInSequence < _currentQTESequence.ListSubHandlers.Count - 1)
                    {
                        OnInputCorrect?.Invoke();
                        _indexInSequence++;
                    } else
                    {
                        OnInputCorrect?.Invoke();
                    }
                } else
                {
                    _indexInSequence = 0;
                }
            }
            yield return null;
            
        }
    }
    IEnumerator StartRoutineSimultaneous()
    {
        yield return null;
    }
}
