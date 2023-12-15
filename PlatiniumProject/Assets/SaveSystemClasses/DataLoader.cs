using KorYmeLibrary.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour, IDataSaveable<GameData>
{
    public float Volume {  get; set; }
    public bool AreRumblesActivated {  get; set; }

    private void Awake()
    {
        if (Globals.DataLoader != null)
        {
            Destroy(gameObject);
            return;
        }
        Globals.DataLoader = this;
    }

    public void InitializeData()
    {
        Volume = 1.0f;
        AreRumblesActivated = true;
    }

    public void LoadData(GameData gameData)
    {
        Volume = gameData.Volume;
        AreRumblesActivated = gameData.AreRumblesActivated;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.Volume = Volume;
        gameData.AreRumblesActivated = AreRumblesActivated;
    }
}
