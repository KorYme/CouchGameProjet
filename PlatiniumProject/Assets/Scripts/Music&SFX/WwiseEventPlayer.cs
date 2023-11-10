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

    public void PlayMusicEvent(WwiseEventEnumMusic music) => _allWwiseEvents?.GetSFXEvent(music.ToString()).Post(gameObject);
    public void PlaySFXEvent(WwiseEventEnumSFX sfx) => _allWwiseEvents?.GetSFXEvent(sfx.ToString()).Post(gameObject);
    public void PlayAnySFX(string str) => _allWwiseEvents?.GetAnyEvent(str).Post(gameObject);
}