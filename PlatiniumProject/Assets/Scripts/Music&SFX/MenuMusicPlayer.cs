using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event _playMenuMusicEvent, _pauseMenuMusicEvent, _resumeMenuMusicEvent, _stopMenuMusicEvent;

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
        PlayMenuMusic();
        (isGamePaused ? _resumeMenuMusicEvent : _pauseMenuMusicEvent)?.Post(gameObject);
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
