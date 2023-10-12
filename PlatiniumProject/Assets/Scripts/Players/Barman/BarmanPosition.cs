using UnityEngine;

public class BarmanPosition : MonoBehaviour
{
    [SerializeField] float _radius;
    [SerializeField] WaitingLineBar _waitingLine;

    public WaitingLineBar WaitingLine { get => _waitingLine;}

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, _radius);
    }
    #endif

}
