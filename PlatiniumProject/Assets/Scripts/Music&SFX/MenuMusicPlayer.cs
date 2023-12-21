using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuMusicPlayer : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event _playMenuMusicEvent, _stopMenuMusicEvent;
    [SerializeField] UnityEvent _onMenuOpen, _onMenuClose;

    public bool IsPlaying { get; private set; } 

    private void Awake()
    {
        if (Globals.MenuMusicPlayer != null)
        {
            Globals.MenuMusicPlayer.PlayMenuMusic();
            Destroy(gameObject);
            return;
        }
        Globals.MenuMusicPlayer = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        IsPlaying = false;
    }

    private IEnumerator Start()
    {
        yield return null;
        PauseOrResumeMusicMenu(Globals.BeatManager == null);
    }

    public void PlayMenuMusic()
    {
        if (!IsPlaying)
        {
            _playMenuMusicEvent?.Post(gameObject);
            IsPlaying = true;
        }
    }

    public void PauseOrResumeMusicMenu(bool isGamePaused)
    {
        if (isGamePaused)
        {
            PlayMenuMusic();
        }
        else
        {
            StopMenuMusic();
        }
        (IsPlaying && Globals.BeatManager != null ? _onMenuOpen : _onMenuClose)?.Invoke();
    }

    public void StopMenuMusic()
    {
        if (IsPlaying)
        {
            _stopMenuMusicEvent?.Post(gameObject);
            IsPlaying = false;
        }
    }
}
