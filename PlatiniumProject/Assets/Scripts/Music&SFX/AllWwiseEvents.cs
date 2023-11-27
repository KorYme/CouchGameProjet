using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WwiseEvents", menuName = "ScriptableObject/WwiseEvents")]
public class AllWwiseEvents : ScriptableObject
{
    [SerializeField, Space] List<AK.Wwise.Event> _allMusicEvents;
    [SerializeField, Space] List<AK.Wwise.Event> _allSFXEvents;

    List<AK.Wwise.Event> _AllEvents => _allMusicEvents.Concat(_allSFXEvents).ToList();

    public List<AK.Wwise.Event> AllMusicEvents => _allMusicEvents;
    public List<AK.Wwise.Event> AllSFXEvents => _allSFXEvents;
    public List<AK.Wwise.Event> AllEvents => _allSFXEvents.Concat(_allMusicEvents).ToList();

    public AK.Wwise.Event GetMusicEvent(string str) => _allMusicEvents.FirstOrDefault(music => music.Name == str);
    public AK.Wwise.Event GetMusicEvent(WwiseEventEnumMusic music) => _allMusicEvents[(int)music];
    public AK.Wwise.Event GetSFXEvent(string str) => _allSFXEvents.FirstOrDefault(sfx => sfx.Name == str);
    public AK.Wwise.Event GetSFXEvent(WwiseEventEnumSFX sfx) => _allSFXEvents[(int)sfx];
    public AK.Wwise.Event GetAnyEvent(string str) => _AllEvents.FirstOrDefault(wwiseEvent => wwiseEvent.Name == str);
}