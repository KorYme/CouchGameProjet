using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public void OpenSceneSync(int sceneIndex) => SceneManager.LoadScene(sceneIndex);
    public void OpenSceneSync(string sceneName) => SceneManager.LoadScene(SceneManager.GetSceneByName(sceneName).buildIndex);
    public void OpenScene(int sceneIndex) => StartCoroutine(LoadScene(sceneIndex));
    public void OpenScene(string sceneName) => StartCoroutine(LoadScene(SceneManager.GetSceneByName(sceneName).buildIndex));
    public void CloseScene(int sceneIndex) => SceneManager.UnloadSceneAsync(sceneIndex);
    public void CloseScene(string sceneName) => SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(sceneName).buildIndex);

    IEnumerator LoadScene(int sceneIndex)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = true;
        yield return null;
    }
}
