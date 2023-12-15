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
            _dictionaryActionSprites[actionSprite.Input] = actionSprite;
        }
    }

    public Sprite GetInput(InputDisplayed input,InputDevice device)
    {
        if (_dictionaryActionSprites.TryGetValue(input, out ActionSprites actionSprite))
        {
            return actionSprite.GetSpriteFromInputDevice(device);
        }
        return null;
    }
}