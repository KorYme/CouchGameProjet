using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public AreaManager areaManager;
    public GameObject character;
    public STARTPOINT startPoint;

    public enum STARTPOINT
    {
        BOUNCER_QUEUE,
        DJ_DANCE_FLOOR,
    }

    SlotInformation FirstSlot
    {
        get
        {
            switch (startPoint)
            {
                case STARTPOINT.BOUNCER_QUEUE:
                    return areaManager.BouncerTransit.Slots[0];
                case STARTPOINT.DJ_DANCE_FLOOR:
                    return areaManager.DjBoard.AvailableSlots[Random.Range(0, areaManager.DjBoard.AvailableSlots.Count)];
                default:
                    return areaManager.BouncerTransit.Slots[0];
            }
        }
    }

    private void Start()
    {
        SpawnATikTak();
    }

    void SpawnATikTak()
    {
        SlotInformation slot = FirstSlot;
        GameObject go = Instantiate(character, slot.transform.position, Quaternion.identity);
        CharacterStateMachine stateMachine = go.GetComponent<CharacterStateMachine>();
        FirstSlot.Occupant = stateMachine;
        if (startPoint == STARTPOINT.DJ_DANCE_FLOOR)
        {
            StartCoroutine(WaitForFirstState(stateMachine, slot));
        }
    }

    IEnumerator WaitForFirstState(CharacterStateMachine stateMachine, SlotInformation slot)
    {
        yield return new WaitUntil(()=> stateMachine.CurrentState != null);
        stateMachine.ChangeState(stateMachine.DancingState);
        stateMachine.CurrentSlot.Occupant = null;
        stateMachine.CurrentSlot = slot;
        stateMachine.CurrentSlot.Occupant = stateMachine;
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && FirstSlot.Occupant == null)
        {
            SpawnATikTak();
        }
    }
}
