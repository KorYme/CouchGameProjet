using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityGoogleDrive;
#if UNITY_EDITOR
using UnityEditor;

public static class WwiseFilesUpdater
{
    const string GOOGLE_DRIVE_FOLDER_ID = "1A6fu8iMUg3yF1vM0rJ04KBgT-MefGeF0";
    static string WwiseSFXFilePath => Path.Combine(Application.dataPath, @"..\", Application.productName + "_WwiseProject", "Originals", "SFX");
    [MenuItem("Tools/Wwise Google Update/Download")]
    public static void Download()
    {
        List<string> localFiles = Directory.GetFiles(WwiseSFXFilePath).ToList();
        GoogleDriveFiles.ListRequest request = new GoogleDriveFiles.ListRequest() { Q = $"'{GOOGLE_DRIVE_FOLDER_ID}' in parents and trashed=false", Fields = new List<string> { "files(name, id)" } };
        request.Send().OnDone += driveFileList =>
        {
            if (request.IsError)
            {
                Debug.LogWarning("The request to look for files in drive ended in an error");
                return;
            }
            driveFileList.Files.ForEach(driveFile =>
            {
                if (Path.GetExtension(driveFile.Name) != ".wav") return;
                if (localFiles.FirstOrDefault(localFile => Path.GetFileName(localFile) == driveFile.Name) == default)
                {
                    Debug.Log($"Dowloading {driveFile.Name}");
                    var downloadRequest = GoogleDriveFiles.Download(driveFile.Id);
                    downloadRequest.Send().OnDone += dlFile =>
                    {
                        if (downloadRequest.IsError)
                        {
                            Debug.LogWarning("The request to download files from the drive ended in an error");
                            return;
                        }
                        File.WriteAllBytes(Path.Combine(WwiseSFXFilePath, driveFile.Name), dlFile.Content);
                        Debug.Log($"Download of {driveFile.Name} done, writing the new file in {WwiseSFXFilePath}");
                    };
                }
            });
        };
        Debug.Log("Download procedure launched");
    }

    [MenuItem("Tools/Wwise Google Update/Upload")]
    public static void Upload()
    {
        List<string> localFiles = Directory.GetFiles(WwiseSFXFilePath).ToList();
        GoogleDriveFiles.ListRequest request = new GoogleDriveFiles.ListRequest() { Q = $"'{GOOGLE_DRIVE_FOLDER_ID}' in parents and trashed=false", Fields = new List<string> { "files(name, id)" } };
        request.Send().OnDone += driveFileList =>
        {
            if (request.IsError)
            {
                Debug.LogWarning("The request to look for files in drive ended in an error"); 
                return;
            }
            localFiles.ForEach(localFile =>
            {
                if (Path.GetExtension(localFile) != ".wav") return;
                string driveFileId = driveFileList.Files.FirstOrDefault(x => Path.GetFileName(localFile) == x.Name)?.Id;
                if (driveFileId == default)
                {
                    Debug.Log($"The file {Path.GetFileName(localFile)} is going to be updated on the drive");
                    var file = new UnityGoogleDrive.Data.File()
                    {
                        Name = Path.GetFileName(localFile),
                        Content = File.ReadAllBytes(Path.Combine(WwiseSFXFilePath, Path.GetFileName(localFile))),
                        Parents = new() { GOOGLE_DRIVE_FOLDER_ID }
                    };
                    var createRequest = GoogleDriveFiles.Create(file);
                    createRequest.Send().OnDone += createdFile =>
                    {
                        if (createRequest.IsError)
                        {
                            Debug.LogWarning("The request to create a file in drive ended in an error");
                            return;
                        }
                        else
                        {
                            Debug.Log($"Upload of {createdFile.Name} done");
                        }
                    };

                }
                else
                {
                    #region DELETE_REQUEST
                    //Debug.Log($"Need to delete a file in the Drive");
                    //GoogleDriveFiles.DeleteRequest deleteRequest = new(driveFileId);
                    //deleteRequest.Send().OnDone += str =>
                    //{
                    //    if (deleteRequest.IsError)
                    //    {
                    //        Debug.LogWarning("The request to delete an old file in drive ended in an error");
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        Debug.Log("The old file was deleted");
                    //    }
                    //    var file = new UnityGoogleDrive.Data.File() { 
                    //        Name = Path.GetFileName(localFile), 
                    //        Content = File.ReadAllBytes(Path.Combine(WwiseSFXFilePath, Path.GetFileName(localFile))), 
                    //        Parents = new() { GOOGLE_DRIVE_FOLDER_ID } 
                    //    };
                    //    var createRequest = GoogleDriveFiles.Create(file);
                    //    createRequest.Send().OnDone += createdFile =>
                    //    {
                    //        if (createRequest.IsError)
                    //        {
                    //            Debug.LogWarning("The request to create a file in drive ended in an error");
                    //            return;
                    //        }
                    //        else
                    //        {
                    //            Debug.Log($"Upload of {createdFile.Name} done");
                    //        }
                    //    };
                    //};
                    #endregion
                    Debug.Log($"A file with the name {Path.GetFileName(localFile)} is already on the drive");
                }
            });
        };
        Debug.Log("Upload procedure Launched");
    }

    [MenuItem("Tools/Wwise Google Update/Authentify")]
    public static void ResetOAuthToken() => AuthController.RefreshAccessToken();
}
#endif
