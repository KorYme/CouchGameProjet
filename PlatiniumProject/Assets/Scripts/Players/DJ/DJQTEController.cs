using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DJQTEController : MonoBehaviour, IQTEable
{
    QTEHandler _qteHandler;
    List<SlotInformation> _shapesLightCopy;
    [SerializeField] private CharacterAnimation _characterAnimation;
    [SerializeField] private CharacterAnimation _characterArmAnimation;
    public int IndexCurrentInput => _qteHandler == null ? 0:_qteHandler.IndexInListSequences;
    #region Events
    public event Action<Sprite[]> OnDJQTEStarted;
    public event Action<Sprite[]> OnDJQTEEnded;
    public event Action<Sprite[]> OnDJQTEChanged;
    [SerializeField] UnityEvent _onDJSuccess;
    [SerializeField] UnityEvent _onDJFail;
    #endregion

    private void Awake()
    {
        _qteHandler = GetComponent<QTEHandler>();
        if(_characterAnimation == null)
            _characterAnimation = GetComponent<CharacterAnimation>();
    }
    private void Start()
    {
        Globals.BeatManager.OnBeatEvent.AddListener(OnBeat);
        if (_qteHandler != null)
        {
            _qteHandler.RegisterListener(this);
        }
    }

    private void OnBeat()
    {
        _characterAnimation.SetAnim(ANIMATION_TYPE.IDLE);
        _characterArmAnimation.SetAnim(ANIMATION_TYPE.IDLE);
    }

    private void OnDestroy()
    {
        Globals.BeatManager.OnBeatEvent.RemoveListener(OnBeat);
        if (_qteHandler != null)
        {
            _qteHandler.UnregisterListener(this);
        }
    }
    //Return the number of players
    private int NbCharactersInLight()
    {        
        return _shapesLightCopy.Count(info => info.Occupant != null);
    }

    private int NbCharactersWithQTEInLight()
    {
        return _shapesLightCopy.Count(information => information.Occupant != null
                && information.Occupant.Satisafaction.CurrentState != CharacterAIStatisfaction.SATISFACTION_STATE.LOYAL
                && information.Occupant.CharacterTypeData.Evilness != Evilness.EVIL);
    }

    public void UpdateQTE(List<SlotInformation> shapesLightCopy)
    {
        _shapesLightCopy = shapesLightCopy;
        if (NbCharactersWithQTEInLight() > 0)
        {
            CharacterTypeData[] clientsData = new CharacterTypeData[NbCharactersInLight()];
            int index = 0;
            //Count the number of characters of each type
            foreach (SlotInformation info in _shapesLightCopy) 
            {
                if (info.Occupant != null)
                {
                    clientsData[index] = info.Occupant.TypeData;
                    index++;
                }
            }
            _qteHandler.StartNewQTE(clientsData);
        }
        else
        {
            _qteHandler.DeleteCurrentCoroutine();
            OnDJQTEEnded?.Invoke(_qteHandler.GetQTESprites());
        }
    }
    #region IQTEable
    public void OnQTEStarted()
    {
        //Debug.Log(_qteHandler.GetQTEString());
        OnDJQTEStarted?.Invoke(_qteHandler.GetQTESprites());
    }

    public void OnQTEComplete()
    {
        OnDJQTEEnded?.Invoke(_qteHandler.GetQTESprites());
    }

    public void OnQTECorrectInput()
    {
        foreach (SlotInformation information in _shapesLightCopy)
        {
            if (information.Occupant != null)
            {
                CharacterStateDancing state = information.Occupant.DancingState as CharacterStateDancing;
                if (state != null)
                {
                    state.OnQTECorrectInput(_qteHandler.LengthInputs);
                }
            }
        }
        OnDJQTEChanged?.Invoke(_qteHandler.GetQTESprites());
        _onDJSuccess?.Invoke();
        _characterAnimation.SetLatency(2);
        _characterArmAnimation.SetLatency(2);
        _characterAnimation.SetAnim(ANIMATION_TYPE.CORRECT_INPUT, false);
        _characterArmAnimation.SetAnim(ANIMATION_TYPE.CORRECT_INPUT, false);
        _characterArmAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.ZWIP);
        _characterArmAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.ECLAIR);
    }

    public void OnQTEWrongInput()
    {
        _onDJFail?.Invoke();
        OnDJQTEChanged?.Invoke(_qteHandler.GetQTESprites());
        _characterAnimation.SetLatency(2);
        _characterArmAnimation.SetLatency(2);
        _characterAnimation.SetAnim(ANIMATION_TYPE.WRONG_INPUT, false);
        _characterArmAnimation.SetAnim(ANIMATION_TYPE.WRONG_INPUT, false);
    }

    public void OnQTEMissedInput()
    {

    }
    #endregion
    public void OnBeginDrop()
    {
        OnDJQTEEnded?.Invoke(_qteHandler.GetQTESprites());
        _qteHandler.PauseQTE(true);
    }

    public void OnDropEnd()
    {
        _qteHandler.PauseQTE(false);
        OnDJQTEStarted?.Invoke(_qteHandler.GetQTESprites());
    }
}
