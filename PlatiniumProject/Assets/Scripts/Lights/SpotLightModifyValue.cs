using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpotLightModifyValue : MonoBehaviour
{
    [SerializeField] Light2D _parentLight;
    [SerializeField] Light2D _childLight;

    private void Reset()
    {
        _parentLight = transform.parent.GetComponent<Light2D>();
        _childLight = GetComponent<Light2D>();
    }

    private IEnumerator Start()
    {
        float initialAngle = _parentLight.pointLightOuterAngle;
        Vector3 initialLocalScale = transform.localScale;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            transform.localScale = initialLocalScale * (_parentLight.pointLightOuterAngle / initialAngle);
        }
    }
}
