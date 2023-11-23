using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadManager : MonoBehaviour
{
    void Start()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }
}
