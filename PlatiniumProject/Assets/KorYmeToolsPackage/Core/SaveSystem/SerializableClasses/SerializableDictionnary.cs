using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    [System.Serializable]
    public class SerializableDictionnary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] List<TKey> _keys = new();
        [SerializeField] List<TValue> _values = new();
        public void OnAfterDeserialize()
        {
            this.Clear();
            if (_keys.Count != _values.Count)
            {
                Debug.LogWarning("There is a different number of keys and values in the save file");
                return;
            }
            for (int i = 0; i < _keys.Count; i++)
            {
                this.Add(_keys[i], _values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }
        }
    }
}
