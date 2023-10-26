using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NameInputDrink : MonoBehaviour
{
    [Serializable]
    public class NameInputConversion
    {
        public string displayName;
        [ActionIdProperty(typeof(RewiredConsts.Action))]
        public int actionId;
    }
    [SerializeField] public NameInputConversion[] _nameInputs;

    public string GetDisplayNameFromAction(int action)
    {
        int i = 0;
        string displayName = "";
        while (i < _nameInputs.Length && displayName != null)
        {
            if (_nameInputs[i].actionId == action)
            {
                displayName = _nameInputs[i].displayName;
            }
        }
        return displayName;
    }

    public int GetActionFromDisplayName(string name)
    {
        int i = 0;
        int action = -1;
        while (i < _nameInputs.Length && action == -1)
        {
            if (_nameInputs[i].displayName == name)
            {
                action = _nameInputs[i].actionId;
            }
        }
        return action;
    }

}
