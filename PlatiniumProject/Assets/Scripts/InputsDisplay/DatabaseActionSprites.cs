using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseActionSprites : MonoBehaviour
{
    [SerializeField] LibraryActionSprites _libraryActionSprites;

    private void Awake()
    {
        Globals.DatabaseActionSprites ??= this;
    }

    public Sprite GetInput(InputDisplayed input, InputDevice device) => _libraryActionSprites == null ? null : _libraryActionSprites.GetInput(input, device);
}
