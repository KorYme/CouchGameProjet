using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class BarmanMovement : MonoBehaviour
{
    [SerializeField] Transform[] _barmanPositions;
    [SerializeField,Range(0f,1f)] float _inputAcceptanceThreshold = 0.1f;
    int _indexPosition;
    [SerializeField] float _timeBetweenBeat = 1f;
    [SerializeField] float _timeBeatAccepted = 0.1f;
    float _timer = 0f;
    [SerializeField] SpriteRenderer _renderer;

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
        _renderer.color = Color.red;
        StartCoroutine(CoroutineBeat());
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
        transform.position = _barmanPositions[_indexPosition].position;
    }

    void ChangeIndexToReach(float value)
    {
        if (value > 1f - _inputAcceptanceThreshold)
        {
            if (_indexPosition < _barmanPositions.Length - 1)
            {
                _indexPosition++;
            }
            MoveBarmanToIndex();
        }
        else if (value < -1f + _inputAcceptanceThreshold)
        {
            if (_indexPosition > 0)
            {
                _indexPosition--;
            }
            MoveBarmanToIndex();
        }
    }

    public void OnMovementInput(CallbackContext context)
    {
        if (context.started)
        {
            if (_timer < _timeBeatAccepted /2f || _timer > _timeBetweenBeat - _timeBeatAccepted / 2f)
            {
                float value = context.ReadValue<float>();
                ChangeIndexToReach(value);
            }
        }
    }
}
