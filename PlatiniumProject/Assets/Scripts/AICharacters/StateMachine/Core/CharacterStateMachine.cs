using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStateMachine : MonoBehaviour
{
    [SerializeField] private CharacterData _characterData;
    private SpawnManager _spawnManager;
    private BeatManager _beatManager;
    public Vector3 PullPos { get; set; }
    public CharacterAIMovement CharacterMove { get; private set; }
    public AreaManager AreaManager { get; private set; }
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
    public CharacterState ExorcizeState { get; } = new CharacterStateExorcize();
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
        DancingState,
        ExorcizeState
    };
    
    #region Propreties
    private CharacterState StartState => IdleTransitState;
    public CharacterState CurrentState { get; private set; }
    public CharacterState PreviousState { get; private set; }
    public CharacterState NextState { get; set; }
    public Vector3 MoveToLocation { get; set; }
    public WaitingLineBar CurrentWaitingLine { get; set; }
    public CharacterData CharacterDataObject
    {
        get { return _characterData; }
        set { if(value != null) _characterData = value; }
    }
    public CharacterTypeData CharacterTypeData
    {
        get { return TypeData; }
        set { if(value != null) TypeData = value; }
    }
    public SlotInformation CurrentSlot { get; set; }
    public int CurrentBeatAmount { get; set; }
    public int CurrentMovementInBouncer { get; set; }
    #endregion

    #region References
    
    public CharacterAIStatisfaction Satisafaction { get; private set; }
    public CharacterAnimation CharacterAnimation { get; private set; }
    public CharacterTypeData TypeData { get; set; }
    public CharacterAiPuller Puller { get; private set; }
    #endregion

    #region Events
    public Action OnCharacterDeath;
    #endregion

    private void Awake()
    {
        InitAllState();
        _beatManager = FindObjectOfType<BeatManager>();
        _spawnManager = FindObjectOfType<SpawnManager>();
        AreaManager = FindObjectOfType<AreaManager>();
        WaitingLines = FindObjectsOfType<WaitingLineBar>();
        CharacterMove = GetComponent<CharacterAIMovement>();
        Satisafaction = GetComponent<CharacterAIStatisfaction>();
        CharacterAnimation = GetComponent<CharacterAnimation>();
        Puller = GetComponent<CharacterAiPuller>();
    }

    public void PullCharacter(CharacterState startState = null)
    {
        CharacterAnimation.SetAnim(ANIMATION_TYPE.IDLE);
        if (startState == null)
        {
            SlotInformation firstQueueSlot = AreaManager.BouncerTransit.Slots[0];
            CurrentSlot = firstQueueSlot;
            transform.position = firstQueueSlot.transform.position;
            ChangeState(IdleTransitState);
            return;
        }
        
        switch (startState)
        {
            case CharacterStateDancing characterStateDancing:
                SlotInformation slotDanceFloor = AreaManager.DjBoard.GetRandomAvailableSlot();

                if (slotDanceFloor == null)
                    return;

                CurrentSlot = slotDanceFloor;
                CurrentSlot.Occupant = this;
                transform.position = slotDanceFloor.transform.position;
                ChangeState(DancingState);
                break;
            
            case CharacterStateIdleTransit characterStateIdleTransit:
                SlotInformation slotQueue = AreaManager.BouncerTransit.GetRandomSlotInQueue();
                
                if (slotQueue == null)
                    return;
                
                CurrentSlot = slotQueue;
                CurrentSlot.Occupant = this;
                transform.position = slotQueue.transform.position;
                ChangeState(IdleTransitState);
                break;
            
            case CharacterStateBarmanQueue characterStateRoam:
                Vector2 destination = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, AreaManager.CircleRadius);
                transform.position = AreaManager.CircleOrigin.position + (Vector3) destination;
                ChangeState(BarManQueueState);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(startState));
        }
    }

    private void Update()
    {
        CurrentState?.UpdateState();
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
        if (CurrentState != null)
        {
            _beatManager.OnBeatEvent.AddListener(CurrentState.OnBeat);
        }
        CurrentState?.EnterState();
    }
    
    private void InitAllState()
    {
        foreach (CharacterState cs in _allState)
        {
            cs.InitState(this);
        }
    }
    public void GoBackInPull()
    {
        _spawnManager.ReInsertCharacterInPull(Puller);
        ChangeState(null);
        CurrentBeatAmount = 0;
        CharacterAnimation.SetColor(Color.white);
        CurrentMovementInBouncer = 0;
        MoveToLocation = Vector3.zero;
        CurrentState = null;
        NextState = null;
    }
}
