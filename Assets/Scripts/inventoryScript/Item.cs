using UnityEngine;

public class Item : MonoBehaviour
{
    // Item Data
    [SerializeField]
    public string itemName;
    
    [SerializeField]
    public int quantity;
    
    [SerializeField]
    public Sprite sprite;

    [TextArea]
    [SerializeField]
    public string itemDescription;

    // Reference to the inventory manager
    private InventoryManager inventoryManager;
    
    void Start()
    {
        // Finds and assigns the InventoryManager from the inventory canvas object
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    // Method is called when anthoer collider enters the objects trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if the object that collided is the player
        if (collision.gameObject.tag == "Player")
        {
            // Adds the item to the player inventory and gets the remaining quantity if there is any
            int leftOverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);
            
            // If no items are left after adding to the inventory, it destroys the game object
            if(leftOverItems <= 0)
                Destroy(gameObject);
            else
                // if some items cant be added, update the quantity to reflect that 
                quantity = leftOverItems;
        }
    }
}
