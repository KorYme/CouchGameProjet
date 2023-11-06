using System.Collections;
using UnityEngine;

public class BarmanMovement : PlayerMovement
{
    [Space, Header("Bouncer Parameters")]
    [SerializeField] BarmanPosition[] _barmanPositions;
    [SerializeField] SpriteRenderer _renderer;

    float _timer = 0f;

    int _indexPosition;
    public int IndexPosition { get => _indexPosition;}

    protected override PlayerRole _playerRole => PlayerRole.Barman;

    private void Awake()
    {
        _indexPosition = 0;
        if (_barmanPositions.Length > 0)
        {
            MoveBarmanToIndex();
        }
    }
    protected override IEnumerator Start()
    {
        DeactivateAllQTE();
        ActivateCurrentQTE();
        
        yield return base.Start();
        Debug.Log("Barman Initialis�");
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
                if (MoveTo(_barmanPositions[_indexPosition+1].transform.position))
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
                if (MoveTo(_barmanPositions[_indexPosition-1].transform.position))
                {
                    DeactivateCurrentQTE();
                    _indexPosition--;
                    ActivateCurrentQTE();
                }
            }
        }
    }


    protected override void OnInputMove(Vector2 vector)
    {
        ChangeIndexToReach(vector.y);
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
