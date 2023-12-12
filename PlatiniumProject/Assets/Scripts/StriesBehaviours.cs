using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StriesBehaviours : MonoBehaviour
{
    [SerializeField] GameObject _stries;

    private void Start()
    {
        Globals.DropManager.OnDropStateChange += CheckState;
    }

    private void OnDestroy()
    {
        Globals.DropManager.OnDropStateChange -= CheckState;
    }

    private void CheckState(DropManager.DROP_STATE state) => _stries?.SetActive(state == DropManager.DROP_STATE.ON_DROP_ALL_PRESSED);

    private void Reset()
    {
        if (transform.childCount <= 0) return;
        _stries = transform.GetChild(0).gameObject;
    }
}
