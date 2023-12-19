using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (_spriteRenderer.material.HasFloat("_NbColors"))
                _nbColors = (int)_spriteRenderer.material.GetFloat("_NbColors");
            _materialDanceFloor = _spriteRenderer.material;
        }
    }

    private void Start()
    {
        _beatManager = Globals.BeatManager;
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

    public void ChangeColor(bool isToggle)
    {
        if (_spriteRenderer != null && _useShader && !isToggle)
        {
            _spriteRenderer.material = _materialDanceFloor;
            _spriteRenderer.color = Color.white;
        }
        else
        {
            _spriteRenderer.material = _materialTileEnlighten;
            _spriteRenderer.color = Color.green;
        }
        _isLit = isToggle;
    }

    private void ChangeOnBeat()
    {
        if (_nbColors > 0)
        {
            _index = (_index + 1) % _nbColors;
        }
        _spriteRenderer.material.SetFloat("_ChangeColorsPosition", _index);
    }
}
