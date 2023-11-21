using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraProfileManager : MonoBehaviour
{
    private CameraProfile[] _cameraProfiles;
    private void Awake()
    {
        Globals.CameraProfileManager ??= this;
        _cameraProfiles = FindObjectsOfType<CameraProfile>();
    }

    private void Start()
    {
        for (int i = 0; i < _cameraProfiles.Length; ++i)
        {
            if (i % 2 == 0)
            {
                _cameraProfiles[i].StartShake(10f, 5f, .1f);
            }
        }
    }
}
