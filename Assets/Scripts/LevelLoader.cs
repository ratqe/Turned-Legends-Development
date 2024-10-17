using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    // Reference to the loading screen
    public GameObject loadingScreen;

    // Reference to the slider
    public Slider progressBar;

    // Minimum time the loading screen shows
    public float minimumLoadingTime = 2f;

    // Method to initiate the level loading process
    public void LoadLevel(int sceneIndex)
    {
        // Starting the asynchronous scene 
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    // Coroutine to handles asynchronous loading of a scene
    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        // make the loading screen visible
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Start the loading of the scene, but dont show it yet
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        // Storing the starting time of the loading process
        float startTime = Time.time;

        // Progress simulated based on the minimum loading time
        float simulatedProgress = 0f;

        // While loop to loop until the scene is done loading
        while (!operation.isDone)
        {
            // Calculate elasped time since the loading started
            float elapsedTime = Time.time - startTime;

            // Simulate progress using the elapsed time and minimum time
            simulatedProgress = Mathf.Clamp01(elapsedTime / minimumLoadingTime);

            // The real progress of the operation
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Chooses the lower of the simulated and real progress to display
            float displayProgress = Mathf.Min(simulatedProgress, realProgress);

            // If the progress bar exists, update it
            if (progressBar != null)
            {
                progressBar.value = displayProgress;
            }

            // if the scene is ready and the minimum time has passed, show the loaded scene
            if (operation.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
        
        // when the scene is loaded, hide the loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
}
