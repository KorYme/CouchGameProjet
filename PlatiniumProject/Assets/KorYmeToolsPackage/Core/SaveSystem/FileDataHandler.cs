using System;
using System.IO;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    public class FileDataHandler<T>
    {
        #region FIELDS
        private string _dataDirPath;
        private string _dataFileName;
        private EncryptionUtilities.EncryptionType _encryptionType;
        private string _encryptionString;
        #endregion

        #region PROPERTIES
        string _fullPath
        {
            get => Path.Combine(_dataDirPath, _dataFileName);
        }
        #endregion

        #region CONSTRUCTORS
        public FileDataHandler(string dataDirPath = "", string dataFileName = "", EncryptionUtilities.EncryptionType encryptionType = EncryptionUtilities.EncryptionType.None, string encryptionString = "")
        {
            _dataDirPath = dataDirPath;
            _dataFileName = dataFileName;
            _encryptionType = encryptionType;
            _encryptionString = encryptionString;
        }
        #endregion

        #region METHODS
        public T Load()
        {
            if (!File.Exists(_fullPath)) return default(T);
            try
            {
                string dataToLoad;
                using (FileStream stream = new FileStream(_fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = EncryptionUtilities.Encrypt(reader.ReadToEnd(), _encryptionType, false, _encryptionString);
                    }
                }
                return JsonUtility.FromJson<T>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error occured when trying to save data to file: " + _fullPath + "\n" + e);
                return default(T);
            }
        }

        public void Save(T data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));
                using (FileStream stream = new FileStream(_fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(EncryptionUtilities.Encrypt(JsonUtility.ToJson(data, true), _encryptionType, true, _encryptionString));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + _fullPath + "\n" + e);
            }
        }

        public static void DestroyOldData()
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
            int fileDestroyed = 0;
            int directoryDestroyed = 0;
            foreach (FileInfo file in di.GetFiles())
            {
                Debug.Log("This file has been deleted  : \n" + file.Name);
                file.Delete();
                fileDestroyed++;
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                Debug.Log("This directory has been deleted  : \n" + dir.Name);
                dir.Delete(true);
                directoryDestroyed++;
            }
            Debug.Log("You have destroyed " + fileDestroyed.ToString() + " files and " + directoryDestroyed.ToString() + " directories.");
        }
        #endregion
    }
}
