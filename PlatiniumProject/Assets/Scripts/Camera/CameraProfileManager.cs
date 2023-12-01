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

    public void StartPulseForAll()
    {
        foreach (var cam in _cameraProfiles)
        {
            cam.StartPulseZoom();
        }
    }
    
    public void StartPulseForAllOnce(float percentage, float duration)
    {
        foreach (var cam in _cameraProfiles)
        {
            cam.StartPulseZoom(true, percentage, duration);
        }
    }
    

    public CameraProfile FindCamera(CAMERA_TYPE type)
    {
        return _cameraProfilesDict[type];
    }
}
