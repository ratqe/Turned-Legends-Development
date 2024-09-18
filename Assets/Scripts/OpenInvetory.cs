using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenInvetory : MonoBehaviour
{
    public GameObject InventoryMenu;

    public bool isOpen;

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
            if(isOpen)
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
        // Activate the menu
        InventoryMenu.SetActive(true);
       
        // Stop the in-game Clock
        Time.timeScale = 0f;
        
        isOpen = true;
    }

    public void ResumeGame()
    {
        // Deactivate the menu
        InventoryMenu.SetActive(false);

        // Resume the in-game clock
        Time.timeScale = 1f;
        
        isOpen = false;
    }
}
