using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatBounceJuiceManager : MonoBehaviour
{
    public static BeatBounceJuiceManager instance;
    public List<BeatBounceJuice> BeatBounceList  = new List<BeatBounceJuice>(); 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
