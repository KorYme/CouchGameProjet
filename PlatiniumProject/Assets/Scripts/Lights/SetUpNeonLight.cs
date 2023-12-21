using System.Collections;
using System.Collections.Generic;
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
        Globals.PriestCalculator.OnPriestExorcize += () => gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroy(_parentLineRenderer.material);
        Destroy(_childLineRenderer.material);
        Globals.PriestCalculator.OnPriestExorcize -= () => gameObject.SetActive(false);
    }

    public void SetMaterialColor()
    {
        if (_parentLineRenderer == null) return;
        _parentLineRenderer.material.color = _parentLineColor;
        if (_childLineRenderer == null) return;
        _childLineRenderer.material.color = _childLineColor;
    }

    public void SetUpAllVertices()
    {
        if (_parentLineRenderer == null) return;
        if (_childLineRenderer != null)
        {
            _childLineRenderer.positionCount = _parentLineRenderer.positionCount;
            _childLineRenderer.loop = _parentLineRenderer.loop;
        }
        for (int i = 0; i < _parentLineRenderer.positionCount; i++)
        {
            _parentLineRenderer.SetPosition(i, (Vector2)_parentLineRenderer.GetPosition(i));
            _childLineRenderer?.SetPosition(i, _parentLineRenderer.GetPosition(i));
        }
    }
}