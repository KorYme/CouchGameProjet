using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class VfxHandeler : MonoBehaviour
{
    
    [System.Serializable]
    public struct Vfx
    {
        public VFX_TYPE vfxType;
        public VisualEffect vfx;
    }
    public enum VFX_TYPE
    {
        NO,
        YEAH,
        SHAK,
        ZWIP,
        AMEN,
        CHOC,
        ECLAIR,
        EXCLAMATION,
        RED_IMPACT,
        RED_IMPACT_LIGHT,
        BLUE_IMPACT,
        INCANTATION,
        SATISAFCTION,
        SHAKE,
        SHAKE2,
    }

    [SerializeField] private Vfx[] _vfxs;
    private Dictionary<VFX_TYPE, VisualEffect> _vfxDict = new Dictionary<VFX_TYPE, VisualEffect>();

    private void Awake()
    {
        foreach (var v in _vfxs)
        {
            _vfxDict[v.vfxType] = v.vfx;
        }
    }

    public void PlayVfx(VFX_TYPE type, int vfxCount = 1)
    {
        for (int i = 0; i < vfxCount; i++)
        {
            _vfxDict[type]?.SendEvent("CustomPlay");
        }
    }
}
