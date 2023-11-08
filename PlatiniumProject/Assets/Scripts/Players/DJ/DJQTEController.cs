using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DJQTEController : MonoBehaviour, IQTEable
{
    QTEHandler _qteHandler;
    [SerializeField] TextMeshProUGUI _QTEDisplay;
    [SerializeField] GameObject _bubbleObject;
    List<SlotInformation> _shapesLightCopy;

    private void Awake()
    {
        _qteHandler = GetComponent<QTEHandler>();
    }
    private void Start()
    {
        if (_qteHandler != null)
        {
            _qteHandler.RegisterQTEable(this);
        }
        _QTEDisplay.text = _qteHandler.GetQTEString();
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

    public void UpdateQTE()
    {
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
        }
        _QTEDisplay.text = _qteHandler.GetQTEString();
    }
    public void UpdateShape(List<SlotInformation> shape)
    {
        _shapesLightCopy = shape;
    }

    #region IQTEable
    public void OnQTEStarted()
    {
        _QTEDisplay.text = _qteHandler.GetQTEString();
    }

    public void OnQTEComplete()
    {
        _QTEDisplay.text = _qteHandler.GetQTEString();
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
        _QTEDisplay.text = _qteHandler.GetQTEString();
    }

    public void OnQTEWrongInput()
    {
        _QTEDisplay.text = _qteHandler.GetQTEString();
    }
    #endregion
}
