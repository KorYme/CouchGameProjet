using UnityEditor;
using UnityEngine;

public class WwiseEventPlayer : MonoBehaviour
{
    [SerializeField] AllWwiseEvents _allWwiseEvents;

    private void Reset()
    {
        _allWwiseEvents = (AllWwiseEvents)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/WwiseEvents/WwiseEvents.asset", typeof(AllWwiseEvents));
    }

    public void PlayMusicEvent(WwiseEventEnumMusic music) => _allWwiseEvents?.GetSFXEvent(music.ToString()).Post(gameObject);
    public void PlaySFXEvent(WwiseEventEnumSFX sfx) => _allWwiseEvents?.GetSFXEvent(sfx.ToString()).Post(gameObject);
    public void PlayAnySFX(string str) => _allWwiseEvents?.GetAnyEvent(str).Post(gameObject);
}