using System;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] private CharacterAnimationObject _characterAnimationData;
    private int _animationIndex;
    private ANIMATION_TYPE lastAnimationType;

    public CharacterAnimationObject CharacterAnimationObject
    {
        get { return _characterAnimationData; }
        set { if(value != null) _characterAnimationData = value; }
    }


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
                if (!_characterAnimationData.Animations.ContainsKey(ANIMATION_TYPE.DANCING))
                    return null;
                result = _characterAnimationData.Animations[ANIMATION_TYPE.DANCING].GetNextAnimationSprite(ref _animationIndex);
                if(result == null )
                    throw new ArgumentOutOfRangeException("there is no sprite for animation");
                return result;
            case ANIMATION_TYPE.IDLE:
                if (!_characterAnimationData.Animations.ContainsKey(ANIMATION_TYPE.IDLE))
                    return null;
                result = _characterAnimationData.Animations[ANIMATION_TYPE.IDLE].GetNextAnimationSprite(ref _animationIndex);
                if(result == null)
                    throw new ArgumentOutOfRangeException("there is no sprite for animation");
                return result;
            case ANIMATION_TYPE.EXORCIZE:
                if (!_characterAnimationData.Animations.ContainsKey(ANIMATION_TYPE.EXORCIZE))
                    return null;
                result = _characterAnimationData.Animations[ANIMATION_TYPE.EXORCIZE].GetNextAnimationSprite(ref _animationIndex);
                if(result == null)
                    throw new ArgumentOutOfRangeException("there is no sprite for animation");
                return result;
            case ANIMATION_TYPE.MOVE:
                if (!_characterAnimationData.Animations.ContainsKey(ANIMATION_TYPE.MOVE))
                    return null;
                result = _characterAnimationData.Animations[ANIMATION_TYPE.MOVE].GetNextAnimationSprite(ref _animationIndex);
                if(result == null)
                    throw new ArgumentOutOfRangeException("there is no sprite for animation");
                return result;
            default:
                throw new ArgumentOutOfRangeException(nameof(animation), animation, null);
        }
    }
}
