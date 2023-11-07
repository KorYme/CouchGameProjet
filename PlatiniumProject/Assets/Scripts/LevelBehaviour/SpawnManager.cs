using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private int _objectsToPoolNumber;
    [SerializeField] private Transform _poolingSpawn;
    [SerializeField] private GameObject _pnj;
    private AreaManager _areaManager;

    [Header("Spawn Parameter")] 
    [SerializeField] private Vector2 _minMaxSpawnPerMinutes;
    [SerializeField] private AnimationCurve _spawnEvolutionCurve;
    
    [Tooltip("badclient/Clients ex: 2/10")]
    [SerializeField] private Vector2 _badClientRatio;
    private bool[] _badClientsBools;
    private int _instanciationCount;

    [Header("Bouncer")]
    [SerializeField] private int _baseClientInBouncer;
    [Header("BarMan")]
    [SerializeField] private int _baseClientInBarMan;
    [Header("Dj")]
    [SerializeField] private int _baseClientInDj;
    private CharacterAiPuller[] _characterList;
    private List<CharacterAiPuller> _availableCharcters = new List<CharacterAiPuller>();
    
    [Header("Clients")]
    [SerializeField] private CharacterObject[] _goodClients;
    [SerializeField] private CharacterObject[] _badClients;
    
    public bool CanSpawnClients { get; set; }

    public enum STARTPOINT
    {
        BOUNCER_QUEUE,
        DJ_DANCE_FLOOR,
    }

    private void Awake()
    {
        _areaManager = FindObjectOfType<AreaManager>();
        GdTest();
        
        _badClientsBools = new bool[(int)_badClientRatio.y];
        _characterList = new CharacterAiPuller[_objectsToPoolNumber];
        for (int i = 0; i < _objectsToPoolNumber; ++i)
        {
            GameObject go = Instantiate(_pnj, _poolingSpawn.position, Quaternion.identity);
            go.transform.position += new Vector3(1f, 0f, 0f) * i;
            CharacterAiPuller puller = go.GetComponent<CharacterAiPuller>();
            puller.PullPos = go.transform.position += new Vector3(1f, 0f, 0f) * i;
            _characterList[i] = puller;
            _availableCharcters.Add(puller);
        }
        CanSpawnClients = true;
        Globals.SpawnManager ??= this;
    }

    private void GdTest()
    {
        if (_minMaxSpawnPerMinutes.x <= 0 || _minMaxSpawnPerMinutes.y <= 0)
            Debug.LogException(new DataException("SpawnIntervalle must be positive integer"), this);

        if (_minMaxSpawnPerMinutes.x >= _minMaxSpawnPerMinutes.y)
            Debug.LogException(new DataException("SpawnIntervalle first value must be higher than the second"), this);

        if (_badClientRatio.x <= 0 || _badClientRatio.y <= 0)
            Debug.LogException(new DataException("badclientRation must be positive integer"), this);

        if (_badClientRatio.x >= _badClientRatio.y)
            Debug.LogException(new DataException("badclientRation first value must be higher than the second"), this);
    }

    private void Start()
    {
        for (int i = 0; i < _baseClientInBouncer; ++i)
        {
            if (_availableCharcters.Count <= 0)
            {
                Debug.LogWarning("No more pullable character");
                return;
            }
            CharacterAiPuller chara = _availableCharcters[0];
            _availableCharcters.Remove(chara);
            chara.PullCharacter(GetClientType(), chara.StateMachine.IdleTransitState);
        }
        for (int i = 0; i < _baseClientInBarMan; ++i)
        {
            if (_availableCharcters.Count <= 0)
            {
                Debug.LogWarning("No more pullable character");
                return;
            }
            CharacterAiPuller chara = _availableCharcters[0];
            _availableCharcters.Remove(chara);
            chara.PullCharacter(GetClientType(), chara.StateMachine.BarManQueueState);
        }
        for (int i = 0; i < _baseClientInDj; ++i)
        {
            if (_availableCharcters.Count <= 0)
            {
                Debug.LogWarning("No more pullable character");
                return;
            }
            CharacterAiPuller chara = _availableCharcters[0];
            _availableCharcters.Remove(chara);
            chara.PullCharacter(GetClientType(), chara.StateMachine.DancingState);
        }

        StartCoroutine(PullRoutine());
    }
    
    private CharacterObject GetClientType(bool isBadClient = false)
    {
        return isBadClient? _badClients[Random.Range(0, _badClients.Length)] : _goodClients[Random.Range(0, _goodClients.Length)];
    }
    
    private bool IsBadClient()
    {
        _instanciationCount = (_instanciationCount + 1) % (int)_badClientRatio.y;

        if (_instanciationCount == 0)
        {
            SetClientRatio();
        }
        
        if (_badClientsBools[_instanciationCount]) return true;
        return false;
    }

    private void SetClientRatio()
    {
        List<int> availableIndex = new List<int>();
        for (int i = 0; i < _badClientsBools.Length; ++i)
        {
            _badClientsBools[i] = false;
            availableIndex.Add(i);
        }

        for (int i = 0; i < _badClientRatio.x; ++i)
        {
            int index = Random.Range(0, availableIndex.Count);
            _badClientsBools[index] = true;
            availableIndex.Remove(index);
        }
    }
    public void PullACharacter()
    {
        if (_availableCharcters.Count <= 0 || _areaManager.BouncerTransit.Slots[0].Occupant != null)
        {
            Debug.LogWarning(_availableCharcters.Count <= 0?"No more pullable character" : "First Slot Occuped");
            return;
        }
        CharacterAiPuller chara = _availableCharcters[0];
        chara.StateMachine.CurrentSlot = _areaManager.BouncerTransit.Slots[0];
        chara.StateMachine.CurrentSlot.Occupant = chara.StateMachine;
        _availableCharcters.Remove(chara);
        chara.PullCharacter(GetClientType(IsBadClient()));
    }

    public void ReInsertCharacterInPull(CharacterAiPuller chara)
    {
        _availableCharcters.Add(chara);
        chara.transform.position = chara.PullPos;
    }

    IEnumerator PullRoutine()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= _minMaxSpawnPerMinutes.x)
            {
                yield return new WaitUntil(() => CanSpawnClients && _areaManager.BouncerTransit.Slots[0].Occupant == null);
                timer = 0f;
                PullACharacter();
            }
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PullACharacter();
        }
    }
}
