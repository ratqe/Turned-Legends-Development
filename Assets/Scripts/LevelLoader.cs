using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider progressBar;
    public float minimumLoadingTime = 2f;

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float startTime = Time.time;
        float simulatedProgress = 0f;

        while (!operation.isDone)
        {
            float elapsedTime = Time.time - startTime;
            simulatedProgress = Mathf.Clamp01(elapsedTime / minimumLoadingTime);

            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float displayProgress = Mathf.Min(simulatedProgress, realProgress);

            if (progressBar != null)
            {
                progressBar.value = displayProgress;
            }

            Debug.Log("Display progress: " + displayProgress);

            if (operation.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
}
