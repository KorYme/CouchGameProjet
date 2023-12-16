using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour
{
    [SerializeField] Slider _volumeSlider;
    [SerializeField] Toggle _rumbleToggle;

    private void OnEnable()
    {
        _rumbleToggle.isOn = Globals.DataLoader?.AreRumblesActivated ?? true;
        _volumeSlider.value = Globals.DataLoader?.Volume ?? 1.0f;
    }

    public void UpdateRumbleValue() => Globals.DataLoader.AreRumblesActivated = _rumbleToggle.isOn;
    public void UpdateVolumeValue() => Globals.DataLoader.Volume = _volumeSlider.value;
}
