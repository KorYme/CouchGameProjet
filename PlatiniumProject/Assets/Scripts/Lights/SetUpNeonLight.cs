using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class SetUpNeonLight : MonoBehaviour
{
    [SerializeField] LineRenderer _parentLineRenderer, _childLineRenderer;
    [SerializeField, ColorUsage(true, true)] Color _parentLineColor, _childLineColor;

    private void Reset()
    {
        _parentLineRenderer = GetComponent<LineRenderer>();
        if (transform.childCount == 0) return;
        _childLineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _parentLineRenderer.material.color = _parentLineColor;
        _childLineRenderer.material.color = _childLineColor;
    }

    private void OnDestroy()
    {
        Destroy(_parentLineRenderer.material);
        Destroy(_childLineRenderer.material);
    }

    public void SetMaterialColor()
    {
        if (_parentLineRenderer == null) return;
        _parentLineRenderer.material.color = _parentLineColor;
        if (_childLineRenderer == null) return;
        _childLineRenderer.material.color = _childLineColor;
    }

    public void SetUpAllVertex()
    {
        if (_parentLineRenderer == null) return;
        for (int i = 0; i < _parentLineRenderer.positionCount; i++)
        {
            _parentLineRenderer.SetPosition(i, (Vector2)_parentLineRenderer.GetPosition(i));
            _childLineRenderer.SetPosition(i, _parentLineRenderer.GetPosition(i));
        } 
    }
}