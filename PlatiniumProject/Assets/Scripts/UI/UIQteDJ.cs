using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQteDJ : MonoBehaviour
{
    #region Renderers
    [Header("Renderers")]
    [SerializeField] SpriteRenderer _rendererCurrentInput;
    [SerializeField] SpriteRenderer _rendererNextInput;
    [SerializeField] SpriteRenderer _rendererNextInput2;
    [SerializeField] SpriteRenderer _rendererTransition;
    #endregion

    [SerializeField] Color _taintNextInput = new Color(0.2641509f, 0.2641509f, 0.2641509f, 0.6941177f);

    #region InitialInfos
    Vector3[] _initialPositions = new Vector3[4];
    Vector3[] _initialScales = new Vector3[4];
    #endregion


}
