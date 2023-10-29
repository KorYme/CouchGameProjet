using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

public class QTELoader : MonoBehaviour
{
    public static QTELoader Instance { get; private set; }
    [SerializeField] private List<QTESequence> _listQTE;
    List<QTESequence> ListQTE => _listQTE;
    

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #if UNITY_EDITOR
    public void LoadQTE()
    {
        string[] fileGuids = AssetDatabase.FindAssets("t:" + typeof(QTESequence));
        if (fileGuids.Length > 0)
        {
            for (int i = 0; i < fileGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(fileGuids[i]);
                _listQTE.Add(AssetDatabase.LoadAssetAtPath<QTESequence>(assetPath));
            }
        }
    }
    #endif

    public QTESequence GetRandomQTE(PlayerRole role = PlayerRole.None)
    {
        List<QTESequence> listQTEForRole;
        if (role != PlayerRole.None)
        {
            listQTEForRole = new List<QTESequence>();
            for (int i = 0;i < _listQTE.Count; i++)
            {
                if (_listQTE[i].PlayerRole == role)
                {
                    listQTEForRole.Add(_listQTE[i]);
                }
            }
            if (listQTEForRole.Count == 0)
            {
                listQTEForRole = _listQTE;
            }
        } else
        {
            listQTEForRole = _listQTE;
        }
        int randomIndex = Random.Range(0, listQTEForRole.Count);
        return listQTEForRole[randomIndex];
    }
}
