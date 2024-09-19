using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;

    public bool isOpen;
    public ItemSlot[] itemSlot;

    // Start is called before the first frame update
    void Start()
    {
        InventoryMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if the user presses the 'E' key
        if(Input.GetKeyDown(KeyCode.E))
        {
            // If the inventory is already open it will close the menu and resume the game
            if(isOpen)
            {
                ResumeGame();
            }
            
            // Otherwise pause the game and open the inventory
            else
            {
                PauseGame();
            }
        }
    }

    // Function to pause the game and open the inventory
    public void PauseGame()
    {
        // Activate the inventory menu UI
        InventoryMenu.SetActive(true);

        // Stop the in-game Clock
        Time.timeScale = 0f;

        isOpen = true;
    }

    // Function to resume the game and close the inventory
    public void ResumeGame()
    {
        // Deactivate the inventory menu UI
        InventoryMenu.SetActive(false);

        // Resume the in-game clock
        Time.timeScale = 1f;

        isOpen = false;
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if(itemSlot[i].isFull == false)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite);
                return;
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }
    }
}