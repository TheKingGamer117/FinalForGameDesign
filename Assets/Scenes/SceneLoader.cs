using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // This method can be called by the UI button
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!asyncLoad.isDone)
        {
            // Here, you can add a loading screen or progress bar updates if desired
            yield return null;
        }

        // Scene loading is complete, you can now perform post-load operations here if needed
    }
}
