using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class BarmanMovement : PlayerMovement
{
    [Space, Header("Barman Parameters")]
    [SerializeField] BarmanPosition[] _barmanPositions;
    [SerializeField] UnityEvent _onBarmanMove;

    int _indexPosition;
    public int IndexPosition { get => _indexPosition;}

    protected override PlayerRole PlayerRole => PlayerRole.Barman;
    
    public bool IsPlayingFullQte { get; set; }
    private bool _isInDrop = false;
    public float CurrentDuration {
        get
        {
            if (_barmanPositions == null || _indexPosition >= _barmanPositions.Length || _barmanPositions[_indexPosition].WaitingLine == null)
                return 0f;
            return _barmanPositions[_indexPosition].WaitingLine.DurationValue;
        }
    }

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
        if (IsPlayingFullQte)
        {
            _animation.SetAnim(ANIMATION_TYPE.CORRECT_INPUT);
            _animation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.SHAKE);
            _animation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.SHAKE2);
            _animation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.SHAK);
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
                    _onBarmanMove?.Invoke();
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
        if (!_isInDrop)
            ChangeIndexToReach(vector.x);
    }

    public void ActivateCurrentQTE()
    {
        _barmanPositions[_indexPosition].WaitingLine.PauseQTE(false);
    }
    
    public void DeactivateCurrentQTE()
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

    protected override void OnBeginDrop()
    {
        _isInDrop = true;
        _barmanPositions[_indexPosition].WaitingLine.PauseQTEForDrop(true);
    }

    protected override void OnDropEnd()
    {
        _isInDrop = false;
        if (_barmanPositions[_indexPosition].WaitingLine.NbCharactersWaiting > 0)
        {
            _barmanPositions[_indexPosition].WaitingLine.PauseQTEForDrop(false);
        }
    }
}
