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

    #region Events
    public event Action<string> OnDJQTEStarted;
    public event Action<string> OnDJQTEEnded;
    public event Action<string> OnDJQTEChanged;
    [SerializeField] UnityEvent _onDJSuccess; 
    [SerializeField] UnityEvent _onDJFail;
    #endregion

    private void Awake()
    {
        _qteHandler = GetComponent<QTEHandler>();
        if(_characterAnimation == null)
            _characterAnimation = GetComponent<CharacterAnimation>();
        //_bubbleObject.SetActive(false);
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
            OnDJQTEEnded?.Invoke(_qteHandler.GetQTEString());
        }
    }
    #region IQTEable
    public void OnQTEStarted()
    {
        OnDJQTEStarted?.Invoke(_qteHandler.GetQTEString());
    }

    public void OnQTEComplete()
    {
        OnDJQTEEnded?.Invoke(_qteHandler.GetQTEString());
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
        OnDJQTEChanged?.Invoke(_qteHandler.GetQTEString());
        _characterAnimation.SetLatency(2);
        _characterArmAnimation.SetLatency(2);
        _characterAnimation.SetAnim(ANIMATION_TYPE.CORRECT_INPUT, false);
        _characterArmAnimation.SetAnim(ANIMATION_TYPE.CORRECT_INPUT, false);
        _characterArmAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.ZWIP);
        _characterArmAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.ECLAIR);
    }

    public void OnQTEWrongInput()
    {
        OnDJQTEChanged?.Invoke(_qteHandler.GetQTEString());
        _characterAnimation.SetLatency(2);
        _characterArmAnimation.SetLatency(2);
        _characterAnimation.SetAnim(ANIMATION_TYPE.WRONG_INPUT, false);
        _characterArmAnimation.SetAnim(ANIMATION_TYPE.WRONG_INPUT, false);
    }

    public void OnQTEMissedInput()
    {

    }
    #endregion
}
