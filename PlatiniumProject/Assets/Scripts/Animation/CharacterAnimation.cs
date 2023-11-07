using System;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public enum ANIMATION_TYPE
    {
        DANCING,
        IDLE,
        MOVING,
    }
    [SerializeField] private CharacterAnimationObject _characterAnimationData;
    private int _animationIndex;
    private ANIMATION_TYPE lastAnimationType;

    public CharacterAnimationObject CharacterAnimationObject => _characterAnimationData;


    public void ResetAnimation()
    {
        _animationIndex = 0;
    }
    public Sprite GetAnimationSprite(ANIMATION_TYPE animation)
    {
        if (animation != lastAnimationType)
        {
            _animationIndex = 0;
            lastAnimationType = animation;
        }

        Sprite result;
        switch (animation)
        {
            case ANIMATION_TYPE.DANCING:
                result = _characterAnimationData.dancingAnimation.GetNextAnimationSprite(ref _animationIndex);
                if(result == null)
                    throw new ArgumentOutOfRangeException("there is no sprite for animation");
                return result;
            case ANIMATION_TYPE.IDLE:
                result = _characterAnimationData.idleAnimation.GetNextAnimationSprite(ref _animationIndex);
                if(result == null)
                    throw new ArgumentOutOfRangeException("there is no sprite for animation");
                return result;
            case ANIMATION_TYPE.MOVING:
                result = _characterAnimationData.walkAnimation.GetNextAnimationSprite(ref _animationIndex);
                if(result == null)
                    throw new ArgumentOutOfRangeException("there is no sprite for animation");
                return result;
            default:
                throw new ArgumentOutOfRangeException(nameof(animation), animation, null);
        }
    }
}
