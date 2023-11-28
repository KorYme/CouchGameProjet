using UnityEditor;
using UnityEngine;

public class WwiseEventPlayer : MonoBehaviour
{
    [SerializeField] AllWwiseEvents _allWwiseEvents;

    #if UNITY_EDITOR
    private void Reset()
    {
        _allWwiseEvents = (AllWwiseEvents)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/WwiseEvents/WwiseEvents.asset", typeof(AllWwiseEvents));
    }
#endif
    [SerializeField] WwiseEventEnumSFX _eventSFX;

    private void Start()
    {
        Players.OnPlayerConnect += x =>
        {
            Debug.Log(_eventSFX.ToString());
            Players.PlayersController[x].RB.OnInputStart += () => PlaySFXEvent(_eventSFX);
        };
    }

    public AK.Wwise.Event GetMusicEvent(WwiseEventEnumMusic music) => _allWwiseEvents?.GetMusicEvent(music);
    public void PlayMusicEvent(WwiseEventEnumMusic music) => GetMusicEvent(music)?.Post(gameObject);
    public AK.Wwise.Event GetSFXEvent(WwiseEventEnumSFX sfx) => _allWwiseEvents?.GetSFXEvent(sfx);
    public void PlaySFXEvent(WwiseEventEnumSFX sfx) => _allWwiseEvents?.GetSFXEvent(sfx)?.Post(gameObject);
    public void PlayAnySFX(string str) => _allWwiseEvents?.GetAnyEvent(str)?.Post(gameObject);
}