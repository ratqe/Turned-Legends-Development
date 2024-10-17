using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;  // Reference to the loading screen GameObject
    public Slider progressBar;        // Reference to a UI Slider for progress
    public float minimumLoadingTime = 2f; // Minimum time (in seconds) the loading screen is shown

    public void LoadLevel(int sceneIndex)
    {
        // Start the asynchronous loading process
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        // Show the loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false; // Prevent scene activation until we're ready

        float startTime = Time.time; // Record the start time of loading
        float simulatedProgress = 0f; // Initial simulated progress

        while (!operation.isDone)
        {
            // Simulate the progress bar filling over the `minimumLoadingTime`
            float elapsedTime = Time.time - startTime;
            simulatedProgress = Mathf.Clamp01(elapsedTime / minimumLoadingTime); // Fill over time

            // The actual loading progress goes from 0.0 to 0.9, so we can combine it with simulated progress
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
            
            // The progress bar will reflect whichever is lower: the simulated progress or the real progress
            float displayProgress = Mathf.Min(simulatedProgress, realProgress);

            // Update the progress bar
            if (progressBar != null)
            {
                progressBar.value = displayProgress;
            }

            // Debugging: log the progress
            Debug.Log("Display progress: " + displayProgress);

            // Once the scene is loaded (progress >= 0.9) and the minimum time has passed, allow activation
            if (operation.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null; // Wait for the next frame
        }

        // Hide the loading screen after the scene is fully loaded
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
}
