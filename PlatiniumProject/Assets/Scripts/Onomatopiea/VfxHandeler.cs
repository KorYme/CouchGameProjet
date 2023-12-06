using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class VfxHandeler : MonoBehaviour
{
    public enum VFX_TYPE
    {
        NO,
        YEAH,
        SHAK,
        ZWIP
    }

    [SerializeField] private VisualEffect _noVfx;
    [SerializeField] private VisualEffect _yeahVfx;
    [SerializeField] private VisualEffect _shakVfx;
    [SerializeField] private VisualEffect _zwipVfx;
    
    public void PlayVfx(VFX_TYPE type)
    {
        switch (type)
        {
            case VFX_TYPE.NO:
                _noVfx.SendEvent("CustomPlay");
                break;
            case VFX_TYPE.YEAH:
                _yeahVfx.SendEvent("CustomPlay");
                break;
            case VFX_TYPE.SHAK:
                _shakVfx.SendEvent("CustomPlay");
                break;
            case VFX_TYPE.ZWIP:
                _zwipVfx.SendEvent("CustomPlay");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
