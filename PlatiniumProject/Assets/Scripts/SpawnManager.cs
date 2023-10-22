using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public AreaManager areaManager;
    public GameObject character;

    private void Start()
    {
        GameObject go = Instantiate(character, areaManager.BouncerTransit.Slots[0].transform.position, quaternion.identity);
        areaManager.BouncerTransit.Slots[0].Occupant = go;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && areaManager.BouncerTransit.Slots[0].Occupant == null)
        {
            Debug.Log("sdsdf");
            GameObject go = Instantiate(character, areaManager.BouncerTransit.Slots[0].transform.position, quaternion.identity);
            areaManager.BouncerTransit.Slots[0].Occupant = go;
        }
    }
}
