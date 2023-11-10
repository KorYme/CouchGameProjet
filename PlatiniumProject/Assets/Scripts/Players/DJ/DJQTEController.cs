using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DJQTEController : MonoBehaviour, IQTEable
{
    QTEHandler _qteHandler;
    List<SlotInformation> _shapesLightCopy;

    #region Events
    public event Action<string> OnDJQTEStarted;
    public event Action<string> OnDJQTEEnded;
    public event Action<string> OnDJQTEChanged;
    #endregion

    private void Awake()
    {
        _qteHandler = GetComponent<QTEHandler>();
        if (_qteHandler != null)
        {
            _qteHandler.RegisterQTEable(this);
        }
    }
    private void OnDestroy()
    {
        if (_qteHandler != null)
        {
            _qteHandler.UnregisterQTEable(this);
        }
    }
    //Return the number of players
    private int NbCharactersInLight()
    {
        int nbPlayers = 0;
        foreach (SlotInformation information in _shapesLightCopy)
        {
            if (information.Occupant != null)
            {
                nbPlayers++;
            }
        }
        return nbPlayers;
    }

    public void UpdateQTE(List<SlotInformation> shapesLightCopy)
    {
        _shapesLightCopy = shapesLightCopy;
        int nbCharactersInLight = NbCharactersInLight();
        if (nbCharactersInLight > 0)
        {
            CharacterTypeData[] clientsData = new CharacterTypeData[nbCharactersInLight];
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
        //_bubbleObject.SetActive(false);
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
    }

    public void OnQTEWrongInput()
    {
        OnDJQTEChanged?.Invoke(_qteHandler.GetQTEString());
    }
    #endregion
}
