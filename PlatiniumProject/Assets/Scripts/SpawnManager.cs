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
        GameObject go = Instantiate(character, FirstSlot.transform.position, Quaternion.identity);
        CharacterStateMachine stateMachine = go.GetComponent<CharacterStateMachine>();
        FirstSlot.Occupant = go.GetComponent<CharacterStateMachine>();
        stateMachine.ChangeState(stateMachine.DancingState);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && areaManager.BouncerTransit.Slots[0].Occupant == null)
        {
            GameObject go = Instantiate(character, areaManager.BouncerTransit.Slots[0].transform.position, Quaternion.identity);
            areaManager.BouncerTransit.Slots[0].Occupant = go.GetComponent<CharacterStateMachine>();
        }
    }
}
