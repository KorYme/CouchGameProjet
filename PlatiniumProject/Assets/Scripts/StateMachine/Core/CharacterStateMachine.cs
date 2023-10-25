using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CharacterStateMachine : MonoBehaviour
{
    [SerializeField] private CharacterData _characterData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    BeatManager _beatManager;
    public IMovable CharacterMove { get; private set; }
    public AreaManager AreaManager { get; private set; }
    public Action fakeBeat;
    public WaitingLineBar[] WaitingLines { get; private set; }
    
    #region States
    public CharacterState IdleTransitState { get; } = new CharacterStateIdleTransit();
    public CharacterState IdleBouncerState { get; } = new CharacterStateIdleBouncer();
    public CharacterState MoveToState { get; } = new CharacterStateMoveTo();
    public CharacterState BouncerCheckState { get; } = new CharacterCheckByBouncerState();
    public CharacterState DieState { get; } = new CharacterDieState();
    public CharacterState RoamState { get; } = new CharacterStateRoam();
    public CharacterState BarManQueueState { get; } = new CharacterStateBarmanQueue();
    public CharacterState BarManAtBar { get; } = new CharacterStateAtBar();
    public CharacterState DancingState { get; } = new CharacterStateDancing();
    #endregion
    private CharacterState[] _allState => new CharacterState[]
    {
        IdleTransitState,
        IdleBouncerState,
        MoveToState,
        BouncerCheckState,
        DieState,
        RoamState,
        BarManQueueState,
        BarManAtBar,
        DancingState
    };
    
    #region Propreties
    private CharacterState StartState => IdleTransitState;
    public CharacterState CurrentState { get; private set; }
    public CharacterState PreviousState { get; private set; }
    public CharacterState NextState { get; set; }
    public Vector3 MoveToLocation { get; set; }
    public WaitingLineBar CurrentWaitingLine { get; set; }
    public CharacterData CharacterDataObject => _characterData;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public SlotInformation CurrentSlot { get; set; }
    public int CurrentBeatAmount { get; set; }
    public int CurrentMovementInBouncer { get; set; }
    #endregion

    private void Awake()
    {
        InitAllState();
        _beatManager = FindObjectOfType<BeatManager>();
        AreaManager = FindObjectOfType<AreaManager>();
        CharacterMove = GetComponent<IMovable>();
    }

    private void Start()
    {
        WaitingLines = FindObjectsOfType<WaitingLineBar>();
        CurrentSlot = AreaManager.BouncerTransit.Slots[0];
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
            _beatManager.OnBeatEvent.RemoveListener(CurrentState.OnBeat);
            fakeBeat -= CurrentState.OnBeat;
        }
        PreviousState = CurrentState;
        CurrentState = state;
        fakeBeat += CurrentState.OnBeat;
        _beatManager.OnBeatEvent.AddListener(CurrentState.OnBeat);
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
