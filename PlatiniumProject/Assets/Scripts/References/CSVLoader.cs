using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class CSVLoader : MonoBehaviour
{
    const int COLUMNS = 3;
    const string NAMEXBOX = "XBOX";
    const string NAMEPlaystation = "Playstation";
    //const int NAMEXBOX = 3;


    [SerializeField] TextAsset _textAssetData;
    
    [Serializable]
    public struct ControllerData
    {
        public string Name;
        public InputDevice Type;

        public ControllerData(string name, InputDevice type)
        {
            Name = name;
            Type = type;
        }
    }

    private Dictionary<string,ControllerData> _datas;

    private void Awake()
    {
        if (Globals.DataControllerType != null)
        {
            Destroy(gameObject);
            return;
        }
        Globals.DataControllerType = this;
        if (_textAssetData != null)
            ReadCSV();
    }

    void ReadCSV()
    {
        string[] data = _textAssetData.text.Split(new string[] { ";", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / COLUMNS - 1;
        _datas = new Dictionary<string, ControllerData>();
        for (int i = 0; i < tableSize; i++)
        {
            _datas[data[COLUMNS * (i + 1) + 1]] = new ControllerData(data[COLUMNS * (i + 1)],GetInputDevice(data[COLUMNS * (i + 1) + 2]));
        }
    }

    InputDevice GetInputDevice(string nameInput)
    {
        if (Regex.IsMatch(nameInput, NAMEPlaystation, RegexOptions.IgnorePatternWhitespace))
        {
            return InputDevice.Playstation;
        }
        /*if (Regex.IsMatch(nameInput, NAMEXBOX, RegexOptions.IgnorePatternWhitespace))
        {
            Debug.Log("XBOX");
        }*/
        return InputDevice.XBox;
    }

    public InputDevice GetInputDeviceFromGUID(string controllerGUID)
    {
        if (_datas.ContainsKey(controllerGUID))
        {
            return _datas[controllerGUID].Type;
        }
        return InputDevice.XBox;
    }
}
