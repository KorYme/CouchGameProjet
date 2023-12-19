using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIReturnButton : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] float _timeTocharge = 0.4f;
    [SerializeField] UnityEvent OnReturnEvent;
    
    float _timer = 0f;
    int _nbPlayersHoldingReturn = 0;
    Coroutine _coroutineReturn;

    private void Awake()
    {
        _image.fillAmount = 0f;
    }
    public void IncrementNbPlayersHolding()
    {
        _nbPlayersHoldingReturn++;
        if (_nbPlayersHoldingReturn > 0 && _coroutineReturn == null)
        {
            _coroutineReturn = StartCoroutine(StartRoutineReturn());
        }
    }
    public void DecrementNbPlayersHolding()
    {
        _nbPlayersHoldingReturn = Mathf.Max(_nbPlayersHoldingReturn - 1, 0);
        if (_nbPlayersHoldingReturn == 0 && _coroutineReturn != null)
        {
            StopCoroutine(_coroutineReturn);
            _coroutineReturn = null;
            _timer = 0f;
            _image.fillAmount = _timer / _timeTocharge;
        }
    }

    IEnumerator StartRoutineReturn()
    {
        _timer = 0f;
        while (_timer < _timeTocharge)
        {
            _timer = Mathf.Min(_timer + Time.deltaTime, _timeTocharge);
            _image.fillAmount = _timer / _timeTocharge;
            yield return null;
        }
        _image.fillAmount = _timer / _timeTocharge;
        OnReturnEvent.Invoke();
    }
}
