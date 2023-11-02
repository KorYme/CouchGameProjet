using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SlotRenderer : MonoBehaviour
{
    ITimingable _beatManager;
    SpriteRenderer _spriteRenderer;
    [Header("Lit")]
    [SerializeField] Material _materialTileEnlighten;

    [Header("Not lit")]
    [SerializeField] Material _materialDanceFloorShared;
    Material _materialDanceFloor;
    public Vector2 _position;
    int _nbColors = 0;
    int _index = 0;
    public bool _useShader = false;
    bool _isLit = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_useShader)
        {
            _spriteRenderer.material = _materialDanceFloorShared;
            _spriteRenderer.material.SetVector("_Dimension", new Vector2(0.9f, 0.9f));
            _spriteRenderer.material.SetVector("_Offset", new Vector2(_position.x + 0.05f, _position.y + 0.05f));
            _nbColors = (int)_spriteRenderer.material.GetFloat("_NbColors");
            _materialDanceFloor = _spriteRenderer.material;
        }
        ChangeColor(false);
    }

    private void Start()
    {
        _beatManager = Globals.BeatTimer;
        if (_useShader)
        {
            _beatManager.OnBeatEvent.AddListener(() => ChangeOnBeat()); 
        }
        
    }
    public void SetUpMaterialDancefloor(int i,int j,bool useShader)
    {
        _position = new Vector2(i, j);
        _useShader = useShader;
    }

    public void ChangeColor(bool isEnlighten)
    {
        if (_spriteRenderer != null && _useShader)
        {
            if (isEnlighten)
            {
                _spriteRenderer.material = _materialTileEnlighten;
                _spriteRenderer.color = Color.red;
            }
            else
            {
                _spriteRenderer.material = _materialDanceFloor;
                _spriteRenderer.color = Color.white;
            }
        }
        _isLit = isEnlighten;
    }

    private void ChangeOnBeat()
    {
        _index = (_index + 1) % _nbColors;
        if (!_isLit)
        {
            _spriteRenderer.material.SetFloat("_ChangeColorsPosition", _index);
        }
    }
}
