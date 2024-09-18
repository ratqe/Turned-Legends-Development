using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class mainmenu : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(LoadScene("Lobby", 0.1f)); 
    }

    public void Test()
    {
        StartCoroutine(LoadScene("Battle", 0.1f)); 
    }

    public void GoToSettingMenu()
    {
        StartCoroutine(LoadScene("Setting Menu", 0.1f)); 
    }

    public void GoToMainMenu()
    {
        StartCoroutine(LoadScene("Main Menu", 0.1f)); 
    }

    public void GoToVolumeMenu()
    {
        StartCoroutine(LoadScene("Volume Menu", 0.1f)); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // delay added for the button sound to play before changing scenes
    private IEnumerator LoadScene(string scene, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(scene);
    }
}