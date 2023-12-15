using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct IdActionToInputDisplayed
{
    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int IdAction;
    public InputDisplayed Input;
}

public class DatabaseActionSprites : MonoBehaviour
{
    [SerializeField] LibraryActionSprites _libraryActionSprites;
    [SerializeField] List<IdActionToInputDisplayed> _listActionToInput;
    public Dictionary<int, InputDisplayed> DictionaryActionToInput;

    private void Awake()
    {
        Globals.DatabaseActionSprites ??= this;
        DictionaryActionToInput = new Dictionary<int, InputDisplayed>();
        foreach (IdActionToInputDisplayed converter in _listActionToInput)
        {
            DictionaryActionToInput[converter.IdAction] = converter.Input;
        }
    }

    public Sprite GetInput(InputDisplayed input, InputDevice device) => _libraryActionSprites == null ? null : _libraryActionSprites.GetInput(input, device);
}
