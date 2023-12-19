using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotRenderer : MonoBehaviour
{
    ITimingable _beatManager;
    DropManager _dropManager;
    SpriteRenderer _spriteRenderer;
    [Header("Lit")]
    [SerializeField] Material _materialTileEnlighten;

    [Header("Not lit")]
    [SerializeField] Material _materialDanceFloorShared;
    Material _materialDanceFloor;
    public Vector2 _position;
    int _nbColors = 0;
    int _index = 0;
    [SerializeField] bool _useShader = false;


    [SerializeField] Texture[] _shaderTextures;
    int _indexTexture = 0;

    public bool UseShader { get => _useShader; set => _useShader = value; }

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
        _dropManager = Globals.DropManager;

        if (_useShader)
        {
            _beatManager.OnBeatEvent.AddListener(() => ChangeOnBeat());
            _dropManager.OnDropEnded += OnChangeTexture;
            ChangeColor(false);
        }
    }

    private void OnDestroy()
    {
        if (_useShader)
        {
            _dropManager.OnDropEnded -= OnChangeTexture;
        }
    }
    private void OnChangeTexture()
    {
        if (_useShader && _shaderTextures != null && _shaderTextures.Length > 0)
        {
            _indexTexture = (_indexTexture + 1) % _shaderTextures.Length;
            if (_spriteRenderer.material.HasTexture("_Texture2D"))
            {
                _spriteRenderer.material.SetTexture("_Texture2D", _shaderTextures[_indexTexture]);
            }
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
