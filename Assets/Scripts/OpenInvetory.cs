using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenInvetory : MonoBehaviour
{
    public GameObject InventoryMenu;

    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        InventoryMenu.SetActive(false);
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
        InventoryMenu.SetActive(true); // Activate the menu
        Time.timeScale = 0f; // Stop the in-game clock
        isPaused = true;
    }

    public void ResumeGame()
    {
        InventoryMenu.SetActive(false); // Deactivate the menu
        Time.timeScale = 1f; // Resume the in-game clock
        isPaused = false;
    }
}
