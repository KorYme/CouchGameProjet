using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LibraryActionSprites")]
public class LibraryActionSprites : ScriptableObject
{
    [SerializeField] List<ActionSprites> _listActionSprites;
    Dictionary<InputDisplayed, ActionSprites> _dictionaryActionSprites;
    private void OnEnable()
    {
        _dictionaryActionSprites = new Dictionary<InputDisplayed, ActionSprites>();
        foreach(ActionSprites actionSprite in _listActionSprites)
        {
            _dictionaryActionSprites[actionSprite.Device] = actionSprite;
        }
    }
}
