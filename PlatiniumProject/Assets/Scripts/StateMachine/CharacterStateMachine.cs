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
    BeatManager _beatManager;
    public IMovable CharacterMove { get; private set; }
    public QueueTest qt;
    public Action fakeBeat;
    WaitingLineBar[] _waitingLines;
    
    public CharacterState IdleState { get; } = new CharacterStateIdle();
    public CharacterState MoveToState { get; } = new CharacterStateMoveTo();
    public CharacterState BouncerCheckState { get; } = new CharacterCheckByBouncerState();
    public CharacterState DieState { get; } = new CharacterDieState();
    public CharacterState RoamState { get; } = new CharacterStateRoam();
    public CharacterState BarManQueueState { get; } = new CharacterStateBarmanQueue();
    private CharacterState[] _allState => new CharacterState[]
    {
        IdleState,
        MoveToState,
        BouncerCheckState,
        DieState,
        RoamState,
        BarManQueueState
    };
    
    #region Propreties
    private CharacterState StartState => IdleState;
    public CharacterState CurrentState { get; private set; }
    public CharacterState PreviousState { get; private set; }
    public Vector3 MoveToLocation { get; set; }
    public CharacterData CharacterDataObject => _characterData;
    public SlotInformation[] CurrentTransitQueue { get; set; }
    public SlotInformation CurrentSlot { get; set; }
    public int CurrentBeatAmount { get; set; }
    public Vector2 CurrentChekerBoardId { get; set; }
    public CharacterObjective CharacterCurrentObjective { get; set; } = CharacterObjective.Bouncer;
    public int CurrentMovementInBouncer { get; set; }
    #endregion

    private void Awake()
    {
        InitAllState();
        _beatManager = FindObjectOfType<BeatManager>();
        qt = FindObjectOfType<QueueTest>();
        CharacterMove = GetComponent<IMovable>();
    }

    private void Start()
    {
        _waitingLines = FindObjectsOfType<WaitingLineBar>();
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
    public void ChooseWaitingLine()
    {
        if (_waitingLines.Length > 0) 
        {
            int indexLine = 0;
            int nbCharactersInLine = _waitingLines[0].NbCharactersWaiting;
            
            for (int i = 1; i < _waitingLines.Length; i++)
            {
                if (nbCharactersInLine > _waitingLines[i].NbCharactersWaiting) {
                    nbCharactersInLine = _waitingLines[i].NbCharactersWaiting;
                    indexLine = i;
                }
            }
            _waitingLines[indexLine].AddToWaitingLine(gameObject);
        }
    }

    public void ChangeState(CharacterState state)
    {
        CurrentState?.ExitState();
        if (CurrentState != null)
        {
            _beatManager.OnBeatEvent.RemoveListener(CurrentState.OnBeat);
        }
        PreviousState = CurrentState;
        CurrentState = state;
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
