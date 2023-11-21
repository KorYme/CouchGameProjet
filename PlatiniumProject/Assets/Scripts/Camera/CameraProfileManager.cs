using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraProfileManager : MonoBehaviour
{
    [SerializeField] private CameraProfile[] _cameraProfiles;
    private Dictionary<CAMERA_TYPE, CameraProfile> _cameraProfilesDict = new Dictionary<CAMERA_TYPE, CameraProfile>();
    private void Awake()
    {
        Globals.CameraProfileManager ??= this;
        foreach (var v in _cameraProfiles)
        {
            _cameraProfilesDict[v.CameraType] = v;
        }
    }

    private void Start()
    {
        // for (int i = 0; i < _cameraProfiles.Length; ++i)
        // {
        //     if (i % 2 == 0)
        //     {
        //         _cameraProfiles[i].StartShake(10f, 5f, .1f);
        //     }
        // }
        //
        // _cameraProfiles[1].StartFocus(5f, .5f, GameObject.Find("BouncerP").transform);
    }

    public CameraProfile FindCamera(CAMERA_TYPE type)
    {
        return _cameraProfilesDict[type];
    }
}
