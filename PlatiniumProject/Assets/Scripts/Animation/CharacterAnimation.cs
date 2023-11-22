using System;
using System.Data;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] private CharacterAnimationObject _characterAnimationData;
    private int _animationIndex;
    private ANIMATION_TYPE lastAnimationType;
    [SerializeField] private SpriteRenderer _sp;

    public SpriteRenderer SpriteRenderer => _sp;

    public CharacterAnimationObject CharacterAnimationObject
    {
        get { return _characterAnimationData; }
        set { if(value != null) _characterAnimationData = value; }
    }

    private void Awake()
    {
        _characterAnimationData?.Init();
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
        
        if (!_characterAnimationData.Animations.ContainsKey(animation))
        {
            throw new DataException($"There is no animation of type {animation} in {gameObject.name}");
        }

        Sprite result = _characterAnimationData.Animations[animation].GetNextAnimationSprite(ref _animationIndex);
        if (result == null)
            throw new ArgumentOutOfRangeException("there is no sprite for animation");
        return result;
    }

    public void SetAnim(ANIMATION_TYPE type)
    {
        if(Globals.DropManager.CanYouLetMeMove)
            return;
        _sp.sprite = GetAnimationSprite(type);
    }
}
