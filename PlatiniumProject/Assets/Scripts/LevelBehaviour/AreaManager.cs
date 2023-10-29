using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [Header("Bouncer")]
    [SerializeField] private TransitQueue _bouncerTransit;
    [SerializeField] private CheckerBoard _bouncerBoard;

    [Header("Barman")]
    [SerializeField] private Transform _circleOrigin;
    [SerializeField] private float _circleRadius;

    [Header("DJ")] 
    [SerializeField] private CheckerBoard _djBoard;

    public CheckerBoard BouncerBoard => _bouncerBoard;
    public CheckerBoard DjBoard => _djBoard;
    public TransitQueue BouncerTransit => _bouncerTransit;

    public Transform CircleOrigin => _circleOrigin;
    public float CircleRadius => _circleRadius;
    public List<CharacterStateMachine> RoamQueue { get; set; } = new List<CharacterStateMachine>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_circleOrigin.transform.position, _circleRadius);
    }


}
