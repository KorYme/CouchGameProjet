using UnityEngine;

public class QTEHandler : MonoBehaviour
{
    [SerializeField] PlayerRole _role;
    [SerializeField] PlayerInputController _playerController;
    int _indexInSequence = 0;
    QTESequence _currentQTESequence;

    private void Start()
    {
        ChangeQTE();
    }
    public void ChangeQTE()
    {
        _currentQTESequence = QTELoader.Instance.GetRandomQTE(_role);
        _indexInSequence = 0;
        Debug.Log($"SEQUENCE CHOSEN{_currentQTESequence.Difficulty} {_currentQTESequence.SequenceType} {_currentQTESequence.Index}");
        /*switch(_currentQTESequence.SequenceType)
        {
            case InputsSequence.SEQUENCE:
                
                break;
            case InputsSequence.SIMULTANEOUS:

                break;
        }*/
    }

    public void CheckNextInput()
    {
        UnitInput input = _currentQTESequence.ListSubHandlers[_indexInSequence];
        switch(input.Status)
        {
            case InputStatus.PRESS:
                _playerController.GetInput(input.ActionIndex);
            break;
        }
    }
}
