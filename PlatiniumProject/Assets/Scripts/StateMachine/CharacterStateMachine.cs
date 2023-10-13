using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CharacterStateMachine : MonoBehaviour
{
    public enum CharacterObjective
    {
        None,
        
        Bouncer,
        BarTender,
        DanceFloor,
        Leave
    }
    [SerializeField] private CharacterData _characterData;
    public IMovable CharacterMove { get; private set; }
    public QueueTest qt;
    public Action fakeBeat;
    
    public CharacterState IdleState { get; } = new CharacterStateIdle();
    public CharacterState MoveToState { get; } = new CharacterStateMoveTo();
    public CharacterState BouncerCheckState { get; } = new CharacterCheckByBouncerState();
    public CharacterState DieState { get; } = new CharacterDieState();
    private CharacterState[] _allState => new CharacterState[]
    {
        IdleState,
        MoveToState,
        BouncerCheckState,
        DieState
    };
    
    #region Propreties
    private CharacterState StartState => IdleState;
    public CharacterState CurrentState { get; private set; }
    public CharacterState PreviousState { get; private set; }
    public Transform MoveToLocation { get; set; }
    public CharacterData CharacterDataObject => _characterData;
    public SlotInformation[] CurrentTransitQueue { get; set; }
    public SlotInformation CurrentSlot { get; set; }
    public int CurrentBeatAmount { get; set; }
    public Vector2 CurrentChekerBoardId { get; set; }
    public CharacterObjective CharacterCurrentObjective { get; set; } = CharacterObjective.Bouncer;
    #endregion

    private void Awake()
    {
        InitAllState();
        CharacterMove = GetComponent<IMovable>();
    }

    private void Start()
    {
        CurrentTransitQueue = qt.Transit;
        CurrentSlot = CurrentTransitQueue[0];
        ChangeState(StartState);
    }

    private void Update()
    {
        CurrentState?.UpdateState();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fakeBeat?.Invoke();
        }
    }

    public void ChangeState(CharacterState state)
    {
        CurrentState?.ExitState();
        if (CurrentState != null)
        {
            fakeBeat -= CurrentState.OnBeat;
        }
        PreviousState = CurrentState;
        CurrentState = state;
        fakeBeat += CurrentState.OnBeat;
        CurrentState?.EnterState();
    }
    
    private void InitAllState()
    {
        foreach (CharacterState cs in _allState)
        {
            cs.InitState(this);
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    
}
