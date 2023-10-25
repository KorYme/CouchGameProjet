using Rewired;
using RewiredConsts;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class QTEHandler : MonoBehaviour
{
    [SerializeField] PlayerRole _role;
    [SerializeField] PlayerInputController _playerController;
    int _indexInSequence = 0;
    QTESequence _currentQTESequence;
    [SerializeField] BeatManager _beatManager;
    public event System.Action OnInputCorrect;
    public event System.Action OnSequenceComplete;
    public event Action<UnitInput,int> OnStartQTE;
    private Coroutine _coroutineQTE;

    private void Start()
    {
        GetRandomQTE();
        //_beatManager = FindObjectOfType<BeatManager>();
    }
    public void GetRandomQTE()
    {
        _currentQTESequence = QTELoader.Instance.GetRandomQTE(_role);
        _indexInSequence = 0;
        switch (_currentQTESequence.SequenceType)
        {
            case InputsSequence.SEQUENCE:
                _coroutineQTE = StartCoroutine(StartRoutineSequence());
                break;
            case InputsSequence.SIMULTANEOUS:
                //TO DO
                break;
        }
    }
    
    public void StopCoroutine()
    {
        StopCoroutine(_coroutineQTE);
        _coroutineQTE = null;
    }

    public string DisplayQTE()
    {
        if (_currentQTESequence != null)
        {
            StringBuilder str = new StringBuilder();
            foreach (UnitInput input in _currentQTESequence.ListSubHandlers)
            {
                InputAction action = ReInput.mapping.GetAction(input.ActionIndex);
                if (action != null)
                {
                    str.Append(action.descriptiveName);
                    str.Append(" ");
                }
            }
            str.Append(_indexInSequence.ToString());
            return str.ToString();
        }
        return String.Empty;
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
                    Debug.Log("YES");
                    _indexInSequence++;
                    if (_indexInSequence < _currentQTESequence.ListSubHandlers.Count - 1) 
                    {
                        OnInputCorrect?.Invoke();
                        
                    } else //Sequence finished
                    {
                        OnInputCorrect?.Invoke();
                        OnSequenceComplete?.Invoke();
                        _currentQTESequence = null;
                    }
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
