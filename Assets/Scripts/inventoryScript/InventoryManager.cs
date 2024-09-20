using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // UI menu for inventory
    public GameObject InventoryMenu;

    // Tracks if the inventory menu is opened or closed
    public bool isOpen;

    // Array to hold item slots
    public ItemSlot[] itemSlot;

    // Start is called before the first frame update
    void Start()
    {
        // Make sure the inventory isnt visible when game starts
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

    // Adds item to inventory
    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // First tries to add the item to an existing slot with the same item name
        foreach (ItemSlot slot in itemSlot)
        {
            if (slot.itemName == itemName && slot.isFull == false)
            {
                int leftOverItems = slot.AddItem(itemName, quantity, itemSprite, itemDescription);
                return leftOverItems;
            }
        }

        // If no existing stack, add to an empty slot
        foreach (ItemSlot slot in itemSlot)
        {
            if (!slot.isFull)
            {
                slot.AddItem(itemName, quantity, itemSprite, itemDescription);
                return 0;
            }
        }
        // If inventory is full, return the leftover quantity
        return quantity;
    }


    // Method to deselect all item slots
    public void DeselectAllSlots()
    {
        // Goes through each slot to deselect it 
        for (int i = 0; i < itemSlot.Length; i++)
        {
            // Disable the sharder effect
            itemSlot[i].selectedShader.SetActive(false);
            
            // Marks item as not selected
            itemSlot[i].thisItemSelected = false;
        }
    }
}