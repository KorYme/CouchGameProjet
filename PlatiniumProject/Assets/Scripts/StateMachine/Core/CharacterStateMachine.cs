using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class CharacterStateMachine : MonoBehaviour
{
    [SerializeField] private CharacterData _characterData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private SpawnManager _spawnManager;
    private BeatManager _beatManager;
    public IMovable CharacterMove { get; private set; }
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
        _spawnManager = FindObjectOfType<SpawnManager>();
        AreaManager = FindObjectOfType<AreaManager>();
        WaitingLines = FindObjectsOfType<WaitingLineBar>();
        CharacterMove = GetComponent<IMovable>();
    }

    public void PullCharacter(CharacterState startState = null)
    {
        if (startState == null)
        {
            SlotInformation firstQueueSlot = AreaManager.BouncerTransit.Slots[0];
            CurrentSlot = firstQueueSlot;
            EditorGUIUtility.PingObject(gameObject);
            Debug.Log("zzezz");
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
            
            case CharacterStateRoam characterStateRoam:
                Vector2 destination = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, AreaManager.CircleRadius);
                transform.position = destination;
                ChangeState(RoamState);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(startState));
        }
        // CurrentSlot = AreaManager.BouncerTransit.Slots[0];
        // ChangeState(StartState);
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
        _spawnManager.ReInsertCharacterInPull(this);
        ChangeState(null);
        _spriteRenderer.color = Color.white;
    }
}
