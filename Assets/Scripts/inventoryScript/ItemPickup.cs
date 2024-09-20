using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private InventoryManager inventoryManager;

    private void Start()
    {
        // Get the InventoryManager component
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Add the item back to the player's inventory
            Item item = GetComponent<Item>();

            // Attempt to add item to existing stack
            int leftOverItems = inventoryManager.AddItem(item.itemName, item.quantity, item.sprite, item.itemDescription);
            
            // If no leftover items, destroy the dropped item
            if (leftOverItems <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                // Set the new quantity to the leftover amount
                item.quantity = leftOverItems;
            }
        }
    }
}