using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(fileName ="ListSpritesPlayerTag",menuName = "ScriptableObject/SpritesPlayerTag")]
public class ListPlayerTag : ScriptableObject
{
    [SerializeField] List<Sprite> _playerTagSprites = new List<Sprite>();
    public ReadOnlyCollection<Sprite> PlayerTagSprites => _playerTagSprites.AsReadOnly();
}
