using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace KorYmeLibrary.SaveSystem 
{
   public class DSM_GameData : DataSaveManager<GameData>
   {
        // Modify if you're willing to add some behaviour to the component
    }

   [System.Serializable]
   public class GameData : GameDataTemplate
   {
        // Create the values you want to save here
        public float GeneralVolume, MusicVolume, SFXVolume;
        public bool AreRumblesActivated;
   }
}