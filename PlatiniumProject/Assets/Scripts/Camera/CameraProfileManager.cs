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
    

    public CameraProfile FindCamera(CAMERA_TYPE type)
    {
        return _cameraProfilesDict[type];
    }
}
