using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpotLightSetup : MonoBehaviour
{
    [SerializeField] Light2D _parentLight;

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
