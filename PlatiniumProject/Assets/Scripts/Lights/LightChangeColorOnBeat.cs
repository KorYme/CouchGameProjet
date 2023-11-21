using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightChangeColorOnBeat : MonoBehaviour
{
    [SerializeField] List<Light2D> _lights2D;
    [SerializeField] List<Color> _colors = new List<Color>();


    private void Reset()
    {
        SetUpLights();
    }

    public void SetUpLights()
    {
        _lights2D.Add(GetComponent<Light2D>());
        foreach (var item in GetComponentsInChildren<Light2D>())
        {
            _lights2D.Add(item);
        }
    }

    private void Start()
    {
        Globals.BeatManager.OnBeatEvent.AddListener(ChangeToRandomColor);
    }

    private void OnDestroy()
    {
        Globals.BeatManager.OnBeatEvent.RemoveListener(ChangeToRandomColor);
    }

    private void ChangeToRandomColor()
    {
        if (_colors.Count == 0 || _lights2D.Count == 0) return;
        Color color = _colors[Random.Range(0, _colors.Count - 1)];
        foreach (var item in _lights2D)
        {
            if (item == null) continue;
            item.color = color;
        }
    }
}
