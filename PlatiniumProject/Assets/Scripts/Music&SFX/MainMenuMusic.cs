using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event _playMenuMusicEvent, _stopMenuMusicEvent;

    public bool IsPlaying { get; private set; } 

    private void Awake()
    {
        if (Globals.MainMenuMusic != null)
        {
            Globals.MainMenuMusic.PlayMenuMusic();
            Destroy(gameObject);
            return;
        }
        Globals.MainMenuMusic = this;
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        yield return null;
        IsPlaying = false;
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        if (!IsPlaying)
        {
            _playMenuMusicEvent?.Post(gameObject);
            IsPlaying = true;
        }
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
