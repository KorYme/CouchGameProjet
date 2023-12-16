using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour
{
    [SerializeField] Slider _generalVolumeSlider, _musicVolumeSlider, _sfxVolumeSlider;
    [SerializeField] Toggle _rumbleToggle;

    private void OnEnable()
    {
        _rumbleToggle.isOn = Globals.DataLoader?.AreRumblesActivated ?? true;
        _generalVolumeSlider.value = Globals.DataLoader?.GeneralVolume ?? .5f;
        _musicVolumeSlider.value = Globals.DataLoader?.MusicVolume ?? .5f;
        _sfxVolumeSlider.value = Globals.DataLoader?.SFXVolume ?? .5f;
    }

    public void UpdateRumbleValue() => Globals.DataLoader.AreRumblesActivated = _rumbleToggle.isOn;
    public void UpdateGeneralVolumeValue() => Globals.DataLoader.GeneralVolume = _generalVolumeSlider.value;
    public void UpdateMusicVolumeValue() => Globals.DataLoader.MusicVolume = _musicVolumeSlider.value;
    public void UpdateSFXVolumeValue() => Globals.DataLoader.SFXVolume = _sfxVolumeSlider.value;
}
