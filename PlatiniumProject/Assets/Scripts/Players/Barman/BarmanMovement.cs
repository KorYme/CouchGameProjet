using System.Collections;
using UnityEngine;

public class BarmanMovement : PlayerMovement
{
    [SerializeField] BarmanPosition[] _barmanPositions;
    //[SerializeField,Range(0f,1f)] float _inputAcceptanceThreshold = 0.1f;
    int _indexPosition;
    [SerializeField] float _timeBetweenBeat = 1f;
    [SerializeField] float _timeBeatAccepted = 0.1f;
    float _timer = 0f;
    [SerializeField] SpriteRenderer _renderer;
    bool _inputRefreshed = true;

    [SerializeField] BeatManager _beatManager;

    public int IndexPosition { get => _indexPosition;}
    public BarmanPosition[] BarmanPositions { get => _barmanPositions; }

    private void Awake()
    {
        _indexPosition = 0;
        DeactivateAllQTE();
        ActivateCurrentQTE();
        if (_barmanPositions.Length > 0)
        {
            MoveBarmanToIndex();
        }
    }

    public void MoveBarmanToIndex()
    {
        transform.position = _barmanPositions[_indexPosition].transform.position;
    }

    void ChangeIndexToReach(float value)
    {
        if (value > 0f)
        {
            if (_indexPosition < _barmanPositions.Length - 1)
            {
                if (MoveToPosition(_barmanPositions[_indexPosition+1].transform.position))
                {
                    DeactivateCurrentQTE();
                    _indexPosition++;
                    ActivateCurrentQTE();
                }
            }
        }
        else if (value < 0f)
        {
            if (_indexPosition > 0)
            {
                if (MoveToPosition(_barmanPositions[_indexPosition-1].transform.position))
                {
                    DeactivateCurrentQTE();
                    _indexPosition--;
                    ActivateCurrentQTE();
                }
            }
        }
    }

    protected override IEnumerator Start()
    {
        yield return base.Start();
        _playerController.LeftJoystick.OnInputChange += OnInputMove;
        Debug.Log("Barman Initialisé");
    }

    void OnInputMove()
    {
        ChangeIndexToReach(_playerController.LeftJoystick.InputValue.y);
    }

    void ActivateCurrentQTE()
    {
        _barmanPositions[_indexPosition].WaitingLine.PauseQTE(false);
    }
    
    void DeactivateCurrentQTE()
    {
        _barmanPositions[_indexPosition].WaitingLine.PauseQTE(true);
    }

    void DeactivateAllQTE()
    {
        for (int i = 0; i < _barmanPositions.Length; i++)
        {
            _barmanPositions[i].WaitingLine.PauseQTE(true);
        }
    }
}
