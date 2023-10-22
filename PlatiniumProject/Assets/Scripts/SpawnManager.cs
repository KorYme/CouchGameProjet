using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public QueueTest qt;
    public GameObject character;

    private void Start()
    {
        GameObject go = Instantiate(character, qt.Transit[0].transform.position, quaternion.identity);
        qt.Transit[0].Occupant = go;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && qt.Transit[0].Occupant == null)
        {
            Debug.Log("sdsdf");
            GameObject go = Instantiate(character, qt.Transit[0].transform.position, quaternion.identity);
            qt.Transit[0].Occupant = go;
        }
    }
}
