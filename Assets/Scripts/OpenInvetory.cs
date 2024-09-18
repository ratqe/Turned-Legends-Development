using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenInvetory : MonoBehaviour
{
    public GameObject red;

    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        red.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        red.SetActive(true); // Activate the menu
        Time.timeScale = 0f; // Stop the in-game clock
        isPaused = true;
    }

    public void ResumeGame()
    {
        red.SetActive(false); // Deactivate the menu
        Time.timeScale = 1f; // Resume the in-game clock
        isPaused = false;
    }
}
