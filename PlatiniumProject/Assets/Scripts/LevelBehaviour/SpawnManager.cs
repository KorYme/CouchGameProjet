using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private int _objectsToPoolNumber;
    [SerializeField] private Transform _poolingSpawn;
    [SerializeField] private AreaManager areaManager;
    [SerializeField] GameObject _pnj;

    [Header("Bouncer")]
    [SerializeField] private int _baseClientInBouncer;
    [Header("BarMan")]
    [SerializeField] private int _baseClientInBarMan;
    [Header("Dj")]
    [SerializeField] private int _baseClientInDj;
    private CharacterStateMachine[] _characterList;
    private List<CharacterStateMachine> _availableCharcters = new List<CharacterStateMachine>();
    
    

    public enum STARTPOINT
    {
        BOUNCER_QUEUE,
        DJ_DANCE_FLOOR,
    }

    private void Awake()
    {
        _characterList = new CharacterStateMachine[_objectsToPoolNumber];
        for (int i = 0; i < _objectsToPoolNumber; ++i)
        {
            GameObject go = Instantiate(_pnj, _poolingSpawn.position, Quaternion.identity);
            go.transform.position += new Vector3(1f, 0f, 0f) * i;
            CharacterStateMachine stateMachine = go.GetComponent<CharacterStateMachine>();
            _characterList[i] = stateMachine;
            _availableCharcters.Add(stateMachine);
        }
    }

    private void Start()
    {
        for (int i = 0; i < _baseClientInBouncer; ++i)
        {
            if (_availableCharcters.Count <= 0)
            {
                Debug.LogWarning("No more pullable character");
                return;
            }
            CharacterStateMachine chara = _availableCharcters[0];
            _availableCharcters.Remove(chara);
            chara.PullCharacter(chara.IdleTransitState);
        }
        for (int i = 0; i < _baseClientInBarMan; ++i)
        {
            if (_availableCharcters.Count <= 0)
            {
                Debug.LogWarning("No more pullable character");
                return;
            }
            CharacterStateMachine chara = _availableCharcters[0];
            _availableCharcters.Remove(chara);
            chara.PullCharacter(chara.RoamState);
        }
        for (int i = 0; i < _baseClientInDj; ++i)
        {
            if (_availableCharcters.Count <= 0)
            {
                Debug.LogWarning("No more pullable character");
                return;
            }
            CharacterStateMachine chara = _availableCharcters[0];
            _availableCharcters.Remove(chara);
            chara.PullCharacter(chara.DancingState);
        }
    }

    public void PullACharacter()
    {
        if (_availableCharcters.Count <= 0 || areaManager.BouncerTransit.Slots[0].Occupant != null)
        {
            Debug.LogWarning(_availableCharcters.Count <= 0?"No more pullable character" : "First Slot Occuped");
            return;
        }
        CharacterStateMachine chara = _availableCharcters[0];
        chara.CurrentSlot = areaManager.BouncerTransit.Slots[0];
        chara.CurrentSlot.Occupant = chara;
        _availableCharcters.Remove(chara);
        chara.PullCharacter();
    }

    public void ReInsertCharacterInPull(CharacterStateMachine chara)
    {
        _availableCharcters.Add(chara);
        chara.transform.position = _poolingSpawn.transform.position;
    }

    // void SpawnATikTak()
    // {
    //     SlotInformation slot = FirstSlot;
    //     GameObject go = Instantiate(character, slot.transform.position, Quaternion.identity);
    //     CharacterStateMachine stateMachine = go.GetComponent<CharacterStateMachine>();
    //     FirstSlot.Occupant = stateMachine;
    //     if (startPoint == STARTPOINT.DJ_DANCE_FLOOR)
    //     {
    //         StartCoroutine(WaitForFirstState(stateMachine, slot));
    //     }
    // }
    //
    // IEnumerator WaitForFirstState(CharacterStateMachine stateMachine, SlotInformation slot)
    // {
    //     yield return new WaitUntil(()=> stateMachine.CurrentState != null);
    //     stateMachine.ChangeState(stateMachine.DancingState);
    //     stateMachine.CurrentSlot.Occupant = null;
    //     stateMachine.CurrentSlot = slot;
    //     stateMachine.CurrentSlot.Occupant = stateMachine;
    //     
    // }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PullACharacter();
        }
    }
}
