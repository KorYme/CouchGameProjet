using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BarmanMovement : PlayerMovement
{
    [Space, Header("Barman Parameters")]
    [SerializeField] BarmanPosition[] _barmanPositions;
    [SerializeField] UnityEvent _onBarmanMove;

    int _indexPosition;
    public int IndexPosition { get => _indexPosition;}

    protected override PlayerRole PlayerRole => PlayerRole.Barman;
    
    public bool IsInQte { get; set; }

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
        Debug.Log("Barman Initialisé");
    }

    protected override void OnBeat()
    {
        if (IsInQte)
        {
            _animation.SetAnim(ANIMATION_TYPE.CORRECT_INPUT);
            _animation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.SHAKE);
            _animation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.SHAKE2);
        }
        else
        {
            _animation.SetAnim(ANIMATION_TYPE.IDLE);
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
                    _onBarmanMove?.Invoke();
                    DeactivateCurrentQTE();
                    _indexPosition--;
                    ActivateCurrentQTE();
                }
            }
        }
    }


    protected override void OnInputMove(Vector2 vector)
    {
        ChangeIndexToReach(vector.x);
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
