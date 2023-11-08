using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum ANIMATION_TYPE
{
    DANCING,
    IDLE,
    MOVE,
    EXORCIZE    
}
[System.Serializable]
public struct Animation
{
    public List<Sprite> animationSprites;
    private bool _isActive;
    private ANIMATION_TYPE _animationType;
    
    public ANIMATION_TYPE AnimationType => _animationType;

    public int AnimationLenght => animationSprites.Count;
    public Sprite GetNextAnimationSprite(ref int index)
    {
        if (animationSprites == null || animationSprites.Count <= 0)
            return null;
        index++;
        index %= animationSprites.Count;
        return animationSprites[index];
    }

    public Animation(ANIMATION_TYPE type)
    {
        _animationType = type;
        animationSprites = new List<Sprite>();
        _isActive = false;
    }
    
}

[CreateAssetMenu(fileName ="AnimationData", menuName ="ScriptableObject/AnimationData", order = 2)]
public class CharacterAnimationObject : ScriptableObject
{
    public ANIMATION_TYPE selectedAnimationType;
    public List<Animation> animationsList = new List<Animation>();
    private Dictionary<ANIMATION_TYPE, Animation> _animations = new Dictionary<ANIMATION_TYPE, Animation>();

    public Dictionary<ANIMATION_TYPE, Animation> Animations => _animations;
    
    public void Init()
    {
        foreach (var v in animationsList)
        {
            Animations[v.AnimationType] = v;
        }
    }

    public void AddAnimation()
    {
        if (!animationsList.TrueForAll(x => x.AnimationType != selectedAnimationType))
        {
            Debug.LogWarning($"Animation of type {selectedAnimationType} already exist for this object");
            return;
        }
        Debug.Log($"{selectedAnimationType} added");
        Animation anim = new Animation(selectedAnimationType);
        animationsList.Add(anim);
        _animations[selectedAnimationType] = anim;
        Debug.Log(_animations[selectedAnimationType]);
    }

    public void RemoveAnimation(Animation anim)
    {
            animationsList.Remove(anim);
            _animations.Remove(anim.AnimationType);
    }

    public void ClearAnim()
    {
        animationsList.Clear();
        _animations.Clear();
    }
    

}
