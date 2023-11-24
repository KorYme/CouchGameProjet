using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionManager : MonoBehaviour
{
    [SerializeField] GameObject[] _objectsSelectionable;
    int _indexSelection = 0;

    private IEnumerator Start()
    {
        yield return null;
        
        
    }
}
