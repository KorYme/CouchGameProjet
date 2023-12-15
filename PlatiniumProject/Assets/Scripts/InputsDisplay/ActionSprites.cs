using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public struct ActionSprites
{
    public InputDisplayed Input;
    public Sprite XBoxSprite;
    public Sprite PlaystationSprite;
    public Sprite KB1Sprite;
    public Sprite KB2Sprite;
    public Sprite KB3Sprite;

    public Sprite GetSpriteFromInputDevice(InputDevice device)
    {
        //With the C# version I can't do List + Dictionary instead of switch + not OnEnable in struct
        //So quickest solution was a switch instead of modifying the structure
        Sprite sprite = XBoxSprite;
        switch (device)
        {
            case InputDevice.Playstation:
                if (PlaystationSprite != null)
                    sprite = PlaystationSprite;
                break;
            case InputDevice.Keyboard1:
                if (PlaystationSprite != null)
                    sprite = KB1Sprite;
                break;
            case InputDevice.Keyboard2:
                if (PlaystationSprite != null)
                    sprite = KB2Sprite;
                break;
            case InputDevice.Keyboard3:
                if (PlaystationSprite != null)
                    sprite = KB3Sprite;
                break;
        }
        return sprite;
    }
}
