using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CharacterObject : ScriptableObject
{
    public CharacterData data;
    public CharacterAnimationObject animation;
    public CharacterTypeData type;
    public MovementData movement;
}
