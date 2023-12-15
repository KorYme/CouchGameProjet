using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    public class DataSaveManager<T> : MonoBehaviour where T : GameDataTemplate, new()
    {
        #region FIELDS
        public static DataSaveManager<T> Instance { get; private set; }

        protected T _gameData = null;
        protected List<IDataSaveable<T>> AllSaveData
        {
            get => FindObjectsOfType<MonoBehaviour>().OfType<IDataSaveable<T>>().ToList();
        }
        protected FileDataHandler<T> _fileDataHandler;
        protected bool _dataHasBeenLoaded;

        [Header("File Storage Config")]
        [SerializeField] protected string _fileName;
        [SerializeField] protected EncryptionUtilities.EncryptionType _encryptionType;

        [Header("InGame parameters")]
        [SerializeField] protected bool _saveOnQuit;
        #endregion

        #region METHODS
        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There is more than one DataSaveManager of this type in the scene");
                return;
            }
            Instance = this;
            _fileDataHandler = new FileDataHandler<T>(Application.persistentDataPath, _fileName, _encryptionType);
            _dataHasBeenLoaded = false;
            LoadGame();
        }

        private void Reset()
        {
            _fileName = "data.json";
            _saveOnQuit = true;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        private void OnApplicationQuit()
        {
            if (!_saveOnQuit) return;
            SaveGame();
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        private void OnApplicationFocus(bool focus)
        {
            if (!_saveOnQuit) return;
            if (focus)
            {
                LoadGame();
            }
            else
            {
                SaveGame();
            }
        }
#endif

        public void NewGame()
        {
            _gameData = new T();
            AllSaveData.ForEach(x => x.InitializeData());
        }

        public void LoadGame(bool isLoadForced = false)
        {
            if (_dataHasBeenLoaded && !isLoadForced) return;
            _dataHasBeenLoaded = true;
            _gameData = _fileDataHandler.Load();
            if (_gameData == null)
            {
                Debug.LogWarning("No data was found. Initializing with defaults data.");
                NewGame();
                return;
            }
            AllSaveData.ForEach(x => x.LoadData(_gameData));
        }

        public void SaveGame()
        {
            AllSaveData.ForEach(x => x.SaveData(ref _gameData));
            _fileDataHandler.Save(_gameData);
            _dataHasBeenLoaded = false;
        }

        public void DestroyOldData()
        {
            FileDataHandler<T>.DestroyOldData();
        }
#endregion
    }
}
