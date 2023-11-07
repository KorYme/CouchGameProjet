using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterColor
{
    BLUE, 
    GREEN, 
    YELLOW,
    RED
}

public enum Evilness
{
    GOOD,
    EVIL
}

[CreateAssetMenu()]
public class CharacterTypeData : ScriptableObject
{
    public CharacterColor ClientType;
    public Evilness Evilness;
}
