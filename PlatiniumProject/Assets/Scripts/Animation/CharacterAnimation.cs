using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] private CharacterAnimationObject _characterAnimationData;
    [SerializeField] private SpriteRenderer _sp;
    private Dictionary<ANIMATION_TYPE, int> _animDict = new Dictionary<ANIMATION_TYPE, int>();
    private ANIMATION_TYPE _lastAnimationType;

    public SpriteRenderer SpriteRenderer => _sp;

    public CharacterAnimationObject CharacterAnimationObject
    {
        get { return _characterAnimationData; }
        set { if(value != null) _characterAnimationData = value; }
    }

    private void Awake()
    {
        _characterAnimationData?.Init();
        foreach (var v in _characterAnimationData.animationsList)
        {
            _animDict[v.AnimationType] = 0;
        }
    }
    
    public void ResetAnimation(ANIMATION_TYPE animType)
    {
        _animDict[animType] = 0;
    }
    public void ResetLastAnimation()
    {
        _animDict[_lastAnimationType] = 0;
    }
    public Sprite GetAnimationSprite(ANIMATION_TYPE animation, bool canInterupt)
    {
        if (canInterupt)
        {
            if (animation != _lastAnimationType)
            {
                ResetAnimation(_lastAnimationType);
                _lastAnimationType = animation;
            }
        }
        
        if (!_characterAnimationData.Animations.ContainsKey(animation))
        {
            throw new DataException($"There is no animation of type {animation} in {gameObject.name}");
        }

        int index = _animDict[animation];
        Sprite result = _characterAnimationData.Animations[animation].GetNextAnimationSprite(ref index);
        _animDict[animation] = index;
        
        if (result == null)
            throw new ArgumentOutOfRangeException("there is no sprite for animation");
        return result;
    }

    public void SetAnim(ANIMATION_TYPE type, bool canInterupt = true)
    {
        if(!Globals.DropManager.CanYouLetMeMove)
            return;
        _sp.sprite = GetAnimationSprite(type, canInterupt);
    }
}
