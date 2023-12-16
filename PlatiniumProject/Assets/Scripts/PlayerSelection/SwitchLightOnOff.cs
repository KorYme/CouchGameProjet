using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SwitchLightOnOff : MonoBehaviour
{
    [SerializeField] Light2D _light;
    [SerializeField] PlayerSelectionManager _selectionManager;
    [SerializeField] int _playerIndex = 0;
    [SerializeField] float _timeToTransitionLight = 0.5f;
    float _normalIntensity;

    IEnumerator Start()
    {
        _normalIntensity = _light.intensity;
        SwitchOnOff(false);
        yield return new WaitUntil(() => _selectionManager.IsSetUp);
        if (_selectionManager.PlayersController.Count > _playerIndex)
        {
            //SwitchOnOff(true);
            StartCoroutine(RoutineSwitchOnOff(true));
        }
        _selectionManager.OnPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(int indexPlayer,int indexCharacter)
    {
        if (indexPlayer == _playerIndex)
        {
            StartCoroutine(RoutineSwitchOnOff(true));
        }
    }
    void SwitchOnOff(bool switchLightOn)
    {
        _light.intensity = switchLightOn? _normalIntensity :0f;
    }
    IEnumerator RoutineSwitchOnOff(bool switchLightOn)
    {
        float targetIntensity = switchLightOn ? _normalIntensity :0f;
        float startIntensity = _light.intensity;
        float timer = 0f;
        while (timer < _timeToTransitionLight)
        {
            timer = Mathf.Min(timer + Time.deltaTime, _timeToTransitionLight);
            _light.intensity = Mathf.Lerp(startIntensity, targetIntensity,timer/ _timeToTransitionLight);
            yield return new WaitUntil(() => Globals.BeatManager?.IsPlaying ?? true);
        }
        _light.intensity = targetIntensity;
    }
}
