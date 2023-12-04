using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAnimationBehaviour : MonoBehaviour
{
    public event Action OnDropAnimationClimax;
    public event Action OnDropAnimationEnd;

    public void DropAnimationClimax()
    {
        OnDropAnimationClimax?.Invoke();
    }

    public void DropAnimationEnd()
    {
        OnDropAnimationEnd?.Invoke();
        gameObject.SetActive(false);
    }
}
