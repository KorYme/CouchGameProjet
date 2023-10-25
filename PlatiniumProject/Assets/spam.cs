using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spam : MonoBehaviour
{
    private BeatManager beat;

    private void Awake()
    {
        beat = FindObjectOfType<BeatManager>();
    }
    
    
}
