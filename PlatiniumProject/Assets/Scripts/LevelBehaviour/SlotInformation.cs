using System;
using UnityEngine;

public class SlotInformation : MonoBehaviour
{
    private CharacterStateMachine _occupant;
    public CharacterStateMachine Occupant {
        get {
            return _occupant;
        }
        set { 
            if (_occupant != value)
            {
                _occupant = value;
                OnOccupantChanges?.Invoke();
            }
        } }
    public BouncerMovement PlayerOccupant { get; set; }
    public Sprite BaseSprite { get; private set; }
    public SlotInformation Next { get; set; }
    public int Id { get; set; }
    public SlotInformation[] Neighbours = new SlotInformation[4] {null, null,null, null};

    bool _isEnlighted = false;
    public bool IsEnlighted
    {
        get => _isEnlighted; 
        set
        {
            _isEnlighted = value;
            _fireLight?.SetActive(value);
        }
    }
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SlotRenderer _slotRenderer;
    [SerializeField] private GameObject _fireLight;

    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public SlotRenderer SlotRenderer => _slotRenderer;
    public event Action OnOccupantChanges;

    private void Awake()
    {
        BaseSprite = _spriteRenderer.sprite;
    }

    private void Reset()
    {
        _fireLight = transform.GetChild(0).gameObject;
    }
}
