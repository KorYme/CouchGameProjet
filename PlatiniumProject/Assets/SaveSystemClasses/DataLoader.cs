using KorYmeLibrary.SaveSystem;
using System.Collections;
using System;
using UnityEngine;

public class DataLoader : MonoBehaviour, IDataSaveable<GameData>
{
    [SerializeField] AK.Wwise.RTPC _masterVolumeRTPC, _musicVolumeRTPC, _sfxVolumeRTPC;

    float _generalVolume;
    event Action<float> _onGeneralVolumeChange;
    public float GeneralVolume 
    {
        get => _generalVolume;
        set
        {
            _generalVolume = value;
            _onGeneralVolumeChange?.Invoke(value);
        }
    }

    float _musicVolume;
    event Action<float> _onMusicVolumeChange;
    public float MusicVolume
    {
        get => _musicVolume;
        set
        {
            _musicVolume = value;
            _onMusicVolumeChange?.Invoke(value);
        }
    }

    float _sfxVolume;
    event Action<float> _onSFXVolumeChange;
    public float SFXVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = value;
            _onSFXVolumeChange?.Invoke(value);
        }
    }

    bool _areRumbleActivated;
    public event Action<bool> OnRumbleActivated;
    public bool AreRumblesActivated
    {
        get => _areRumbleActivated;
        set
        {
            _areRumbleActivated = value;
            OnRumbleActivated?.Invoke(value);
        }
    }

    private void Awake()
    {
        if (Globals.DataLoader != null)
        {
            Destroy(gameObject);
            return;
        }
        Globals.DataLoader = this;
    }

    private IEnumerator Start()
    {
        yield return null;
        _onGeneralVolumeChange += _masterVolumeRTPC.SetGlobalValue;
        _onMusicVolumeChange += _musicVolumeRTPC.SetGlobalValue;
        _onSFXVolumeChange += _sfxVolumeRTPC.SetGlobalValue;
        _onGeneralVolumeChange?.Invoke(GeneralVolume);
        _onMusicVolumeChange?.Invoke(MusicVolume);
        _onSFXVolumeChange?.Invoke(SFXVolume);
    }

    public void InitializeData()
    {
        GeneralVolume = .5f;
        MusicVolume = .5f;
        SFXVolume = .5f;
        AreRumblesActivated = true;
    }

    public void LoadData(GameData gameData)
    {
        GeneralVolume = gameData.GeneralVolume;
        MusicVolume = gameData.MusicVolume;
        SFXVolume = gameData.SFXVolume;
        AreRumblesActivated = gameData.AreRumblesActivated;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.GeneralVolume = GeneralVolume;
        gameData.MusicVolume = MusicVolume;
        gameData.SFXVolume = SFXVolume;
        gameData.AreRumblesActivated = AreRumblesActivated;
    }
}
