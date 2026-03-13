using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;

    void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        if (inventoryController == null)
            Debug.LogError("InventoryController not found in scene!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get world item (no tag needed)
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            bool itemAdded = inventoryController.AddItem(collision.gameObject);

            if (itemAdded)

            {
                SoundEffectManager.Instance.Play("Pickup");
                Destroy(collision.gameObject); // remove from world
            }
        }
    }
}