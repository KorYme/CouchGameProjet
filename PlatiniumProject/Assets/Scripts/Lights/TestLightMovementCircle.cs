using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLightMovementCircle : MonoBehaviour
{
    //Circle movements
    [SerializeField, Range(0f, 10f)] float _circleRadius = 1, _speed = 1;

    Vector3 center;

    private IEnumerator Start()
    {
        center = transform.position;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime * Mathf.PI * _speed;
            transform.position = center + new Vector3(Mathf.Cos(timer), Mathf.Sin(timer)) * _circleRadius;
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Application.isPlaying ? center : transform.position, _circleRadius);
    }
}
