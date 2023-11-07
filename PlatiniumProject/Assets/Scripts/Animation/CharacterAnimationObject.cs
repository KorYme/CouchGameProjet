using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Animation
{
    [SerializeField] private Sprite[] _animationSprites;

    public int AnimationLenght => _animationSprites.Length;
    public Sprite GetNextAnimationSprite(ref int index)
    {
        if (_animationSprites == null || _animationSprites.Length <= 0)
            return null;
        index++;
        index %= _animationSprites.Length ;
        return _animationSprites[index];
    }
}

[CreateAssetMenu(fileName ="AnimationData", menuName ="ScriptableObject/AnimationData", order = 2)]
public class CharacterAnimationObject : ScriptableObject
{
    public Animation dancingAnimation;
    public Animation idleAnimation;
    public Animation walkAnimation;
}
