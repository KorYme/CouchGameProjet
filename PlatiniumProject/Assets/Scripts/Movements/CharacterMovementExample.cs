using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovementExample : EntityMovement
{
    [Header("Example parameters")]
    [SerializeField] float _timeToWait = 1f;
    //TEST
    void Start()
    {
        StartCoroutine(TestMovement());
    }

    IEnumerator TestMovement()
    {
        Vector2 dir = Vector2.right;
        while (true)
        {
            MoveTo(dir);
            yield return new WaitWhile(() => IsMoving);
            yield return new WaitForSeconds(_timeToWait);
            dir = new Vector2(-dir.y, dir.x);
        }
    }
}
