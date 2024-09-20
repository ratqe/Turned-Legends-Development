using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // Item Data
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;

    [SerializeField]
    // Max number of items in a slot
    private int maxNumberOfItems;

    // Item Slot
    [SerializeField]
    private TMP_Text quantityText;

    [SerializeField]
    private Image itemImage;

    // Item Descriotion Slot
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    // Shader for highlighting selected items
    public GameObject selectedShader;
    // checks if the item is selected
    public bool thisItemSelected;

    // Reference to the inventory manager
    private InventoryManager inventoryManager;


    private void Start()
    {
        // Finds and assigns the InventoryManager from the inventory canvas object
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    // Method to add a item to a slot
    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // Check if the slot is already full
        if(isFull)
            return quantity;

        // Update Name
        this.itemName = itemName;
        
        // Update Image
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;

        // Update Description
        this.itemDescription = itemDescription;

        // Update Quantity
        this.quantity += quantity;
        if(this.quantity >= maxNumberOfItems)
        {
            // If the quantity is more than the max, cap the slot and return the rest
            quantityText.text = maxNumberOfItems.ToString();
            quantityText.enabled = true;
            
            // Marks slot as full
            isFull = true;
            
            // Return the leftovers
            int extraItems = this.quantity - maxNumberOfItems;
            
            // Set the max quantity
            this.quantity = maxNumberOfItems;
            return extraItems;
        }

        // Update quantity text on the UI
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;

        // return if there are no leftovers
        return 0;
    }

    // Method to handle click events on the item slot
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

        if(eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    // Method for left click
    public void OnLeftClick()
    {
        // Deselect other slots and select only one
        inventoryManager.DeselectAllSlots();
        
        // Highlighs the selected slot
        selectedShader.SetActive(true);
        
        // Marks slot as selected
        thisItemSelected = true;

        // Updates the description for the selected item
        ItemDescriptionNameText.text = itemName;
        ItemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;

        // if the item has no sprite then show an empty sprite
        if(itemDescriptionImage.sprite == null)
        {
            itemDescriptionImage.sprite = emptySprite;
        } 
    }

    // Method to clear the slot
    private void EmptySlot()
    {
        // Disables the quantity text and sets the item to empty sprite
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;

        // Clear the item image and description text
        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
    }

    // Method for right click
   public void OnRightClick()
    {
        // Check if there's any quantity left to drop
        if (this.quantity <= 0)
        {
            Debug.LogWarning("No items left to drop!");
            return;
        }

        // Reduce the item quantity first
        this.quantity -= 1;
        quantityText.text = this.quantity.ToString();

        // If the item quantity is 0, empty the slot
        if (this.quantity <= 0)
        {
            EmptySlot();
        }

        // Create a new item object for dropping
        GameObject itemToDrop = new GameObject(itemName);
        
        // Adds the item component
        Item newItem = itemToDrop.AddComponent<Item>();

        // Set the dropped items properties

        // Always drop 1 unit per right click
        newItem.quantity = 1;
        newItem.itemName = itemName;
        newItem.sprite = itemSprite;
        newItem.itemDescription = itemDescription;

        // Create and modify the sprite renderer
        SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
        sr.sprite = itemSprite;

        // BoxCollider2D set as trigger to make the item interactable
        BoxCollider2D collider = itemToDrop.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;  // Makes sure it doesn't block movement

        // Add the itemPickup script
        itemToDrop.AddComponent<ItemPickup>();

        // Set the location of the dropped item
        itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(1, 0, 0);
        itemToDrop.transform.localScale = new Vector3(.3f, .3f, .3f);
    }
}
