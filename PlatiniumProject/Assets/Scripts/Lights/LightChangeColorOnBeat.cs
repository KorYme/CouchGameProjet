using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightChangeColorOnBeat : MonoBehaviour
{
    [SerializeField] Light2D _light2D;
    [SerializeField] List<Color> _colors = new List<Color>();


    private void Reset()
    {
        _light2D = GetComponent<Light2D>();
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
        if (_colors.Count == 0) return;
        _light2D.color = _colors[Random.Range(0, _colors.Count - 1)];
    }
}
