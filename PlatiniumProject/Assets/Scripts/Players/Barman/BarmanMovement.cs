using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BarmanMovement : MonoBehaviour
{
    PlayerInputController _controller;
    [SerializeField] BarmanPosition[] _barmanPositions;
    //[SerializeField,Range(0f,1f)] float _inputAcceptanceThreshold = 0.1f;
    int _indexPosition;
    [SerializeField] float _timeBetweenBeat = 1f;
    [SerializeField] float _timeBeatAccepted = 0.1f;
    float _timer = 0f;
    [SerializeField] SpriteRenderer _renderer;
    bool _inputRefreshed = true;

    [SerializeField] BeatManager _beatManager;

    public int IndexPosition { get => _indexPosition;}
    public BarmanPosition[] BarmanPositions { get => _barmanPositions;}

    private void Awake()
    {
        _indexPosition = 0;
        if (_barmanPositions.Length > 0)
        {
            MoveBarmanToIndex();
        }
    }
    
    private void Start()
    {
        _renderer.color = _beatManager.IsInsideBeat ? Color.red : Color.blue;
        //StartCoroutine(CoroutineBeat());
        _beatManager.OnBeatStartEvent.AddListener(ChangeColorToRed);
        _beatManager.OnBeatEndEvent.AddListener(ChangeColorToBlue);
    }

    private void OnDestroy()
    {
        _beatManager.OnBeatStartEvent.RemoveListener(ChangeColorToRed);
        _beatManager.OnBeatEndEvent.RemoveListener(ChangeColorToBlue);
    }

    public void ChangeColorToRed()
    {
        _renderer.color = Color.red;
    }
        
    public void ChangeColorToBlue()
    {
        _renderer.color = Color.blue;
    }

    IEnumerator CoroutineBeat()
    {
        while (true)
        {
            _timer += Time.deltaTime;
            _timer %= _timeBetweenBeat;
            
            if (_timer > _timeBetweenBeat - _timeBeatAccepted / 2f)
            {
                _renderer.color = Color.red;
            }
            else if(_timer > _timeBeatAccepted / 2f)
            {
                _renderer.color = Color.blue;
            }

            yield return null;
        }
    }

    public void MoveBarmanToIndex()
    {
        transform.position = _barmanPositions[_indexPosition].transform.position;
    }

    void ChangeIndexToReach(float value)
    {
        DeactivateQTE();
        if (value > 0f)
        {
            if (_indexPosition < _barmanPositions.Length - 1)
            {
                _indexPosition++;
            }
            MoveBarmanToIndex();
            ActivateQTE();
        }
        else if (value < 0f)
        {
            if (_indexPosition > 0)
            {
                _indexPosition--;
            }
            MoveBarmanToIndex();
            ActivateQTE();
        }
    }

    public bool IsInputDuringBeatTime()
    {
        return _beatManager.IsInsideBeat;
    }
    private void Update()
    {
        if (_controller == null)
        {
            SetupController();
        } 
    }

    private void SetupController()
    {
        _controller = Players.PlayersController[(int)PlayerRole.Barman];

        if (_controller != null)
        {
            _controller.LeftJoystick.OnInputStart += OnInputMove;
        }
    }

    void OnInputMove()
    {
        if (_controller != null)
        {
            float value = _controller.LeftJoystick.InputValue.y;
            ChangeIndexToReach(value);
        }

    }

    void ActivateQTE()
    {
        _barmanPositions[_indexPosition].WaitingLine.PauseQTE(false);
    }
    
    void DeactivateQTE()
    {
        _barmanPositions[_indexPosition].WaitingLine.PauseQTE(true);
    }
}
