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
        ANGRY,
        ANGRY2
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

    public void PlayVfx(VFX_TYPE type)
    {
        if (!_vfxDict.ContainsKey(type))
        {
            //Debug.LogWarning("BEBOU LE DICO EST VIDE");
            return;
        }
        _vfxDict[type]?.SendEvent("CustomPlay");
    }

    public void StopVfx(VFX_TYPE type)
    {
        if (!_vfxDict.ContainsKey(type))
        {
            //Debug.LogWarning("BEBOU LE DICO EST VIDE");
            return;
        } 
        _vfxDict[type]?.SendEvent("CustomStop");
    }
}
