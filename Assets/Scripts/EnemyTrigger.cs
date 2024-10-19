using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyTrigger : MonoBehaviour
{
    private BattleTrigger battleTrigger; // Reference to the BattleTrigger script
    private bool isTriggerActive = true; // Control whether the trigger is active

    private void Start()
    {
        battleTrigger = FindObjectOfType<BattleTrigger>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider that entered the trigger belongs to the player and trigger is active
        if (isTriggerActive && other.CompareTag("Player"))
        {
            Debug.Log("Player entered the enemy trigger.");
            if (battleTrigger != null)
            {
                battleTrigger.StartCombat(); // Notify BattleTrigger to start combat
                isTriggerActive = false; // Disable the trigger immediately
            }
        }
    }

    // Method to reset trigger state
    public void ResetTrigger()
    {
        isTriggerActive = true; // Enable the trigger again
    }
}
