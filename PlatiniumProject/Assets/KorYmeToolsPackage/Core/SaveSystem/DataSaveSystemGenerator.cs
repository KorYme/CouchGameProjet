using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace KorYmeLibrary.SaveSystem
{
    public class DataSaveSystemGenerator : MonoBehaviour
    {
        #region FIELDS
        [Header("Parameters")]
        [SerializeField, Tooltip("Name of the class which will countain all the data")] 
        string _dataClassName;
        [SerializeField, Tooltip("Path to the folder starting from the Assets/")]
        string _folderName;
        #endregion

        #region PROPERTIES
        string _folderPath
        {
            get => Application.dataPath + "/" + _folderName;
        }

        string _systemClassName
        {
            get => "DSM_" + _dataClassName;
        }

        string _path
        {
            get => _folderPath + "/" + _systemClassName + ".cs";
        }

        string _classCode
        {
            get =>
                "using System.Collections;" + "\n" +
                "using System.Collections.Generic;" + "\n" +
                "using UnityEngine;" + "\n" +
                "\n" +
                "namespace KorYmeLibrary.SaveSystem " + "\n" +
                "{" + "\n" +
                "   public class " + _systemClassName + " : DataSaveManager<" + _dataClassName + ">" + "\n" +
                "   {" + "\n" +
                "       // Modify if you're willing to add some behaviour to the component" + "\n" +
                "   }" + "\n" +
                "\n" +
                "   [System.Serializable]" + "\n" +
                "   public class " + _dataClassName + " : " + "GameDataTemplate" + "\n" +
                "   {" + "\n" +
                "       // Create the values you want to save here" + "\n" +
                "   }" + "\n" +
                "}";
        }
        #endregion

        #region METHODS
        #if UNITY_EDITOR
        private void Reset()
        {
            _folderName = "SaveSystemClasses";
            _dataClassName = "GameData";
        }

        public void GenerateSaveSystemFolder()
        {
            if (Directory.Exists(_folderPath))
            {
                Debug.LogWarning("A folder named this way already exists in the project.");
                return;
            }
            Directory.CreateDirectory(_folderPath);
            AssetDatabase.Refresh();
        }

        public void GenerateGameDataClass()
        {
            if (!Directory.Exists(_folderPath))
            {
                Debug.LogWarning("No folder named this way has been found in the project. \n" +
                                    "Try creating one with the button above");
                return;
            }
            if (File.Exists(_folderPath + "/" + _systemClassName + ".cs"))
            {
                Debug.LogWarning("There is already one class named this way in " + _folderName);
                return;
            }
            File.WriteAllText(_path, _classCode);
            AssetDatabase.Refresh();
        }

        public void AttachDataSaveManager()
        {
            if (!Directory.Exists(_folderPath))
            {
                Debug.LogWarning("No folder SaveSystemClasses has been found");
                return;
            }
            if (!File.Exists(_folderPath + "/" + _systemClassName + ".cs"))
            {
                Debug.LogWarning("No game data class has been found in the folder : " + _folderPath);
                return;
            }
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes())
                                .FirstOrDefault(t => t.Name == _systemClassName);
            if (type == null)
            {
                Debug.LogWarning("No type has been found");
                return;
            }
            if (GetComponent(type) != null)
            {
                Debug.LogWarning("A component already exists on this gameObject");
                return;
            }
            gameObject.AddComponent(type);
        }
        #endif
        #endregion
    }
}