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
    public QueueTest qt;
    public Action fakeBeat;
    
    public CharacterState IdleState { get; } = new CharacterStateIdle();
    public CharacterState MoveToState { get; } = new CharacterStateMoveTo();
    private CharacterState[] _allState => new CharacterState[]
    {
        IdleState,
        MoveToState,
    };

    private CharacterState StartState => IdleState;
    public CharacterState CurrentState { get; private set; }
    public CharacterState PreviousState { get; private set; }
    public Transform MoveToLocation { get; set; }
    public CharacterData CharacterDataObject => _characterData;
    public SlotInformation[] CurrentTransitQueue { get; private set; }
    public SlotInformation CurrentSlot { get; set; }
    public CharacterObjective CharacterCurrentObjective { get; set; } = CharacterObjective.Bouncer;


    private void Awake()
    {
        InitAllState();
        CurrentTransitQueue = qt.Transit;
    }

    private void Start()
    {
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
    
}
