using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    public GameObject dungeonUI; // Reference to your Dungeon UI GameObject
    public GameObject battleUI;  // Reference to your Battle UI GameObject
    private BattleSystem battleSystem;

    public Vector3 playerPositionBeforeBattle;
    private PlayerControl playerControl; // To store player movement script
    private bool isInCombat = false; // State management for combat

    public CameraFollow cameraFollow;  // Drag the CameraFollow script here

    public Transform playerBattlePosition; // Assign a Transform for player battle position in the inspector
    public Transform enemyBattlePosition;  // Assign a Transform for enemy battle position in the inspector

    public GameObject player;  // Reference to your player GameObject
    public GameObject enemy;   // Reference to your enemy GameObject

    void Start()
    {
        // Find the player's movement script and ensure battle UI is hidden
        playerControl = player.GetComponent<PlayerControl>();
        battleSystem = FindObjectOfType<BattleSystem>();
        battleUI.SetActive(false);  // Battle UI should be hidden initially
    }



    // This function starts combat by switching UI elements
    public void StartCombat()
    {
        // Save player's position before battle
        playerPositionBeforeBattle = player.transform.position;

        if (isInCombat) return; // Prevent starting combat if already in combat

        isInCombat = true; // Set combat state to true

        // Automatically find the nearest enemy if not already assigned
        if (enemy == null)
        {
            enemy = FindNearestEnemy();
        }

        // Other battle initialization code
        cameraFollow.isInCombat = true;  // Stop following the player

        // Disable dungeon UI elements (e.g., map, player controls)
        dungeonUI.SetActive(false);

        // Enable the battle UI
        battleUI.SetActive(true);

        // Disable the player's movement during combat
        playerControl.enabled = false;

        if (battleSystem != null)
        {
            battleSystem.StartBattleFromTrigger();  // Start BattleSystem
        }

        // Optionally, start battle logic here (e.g., turn-based system)
        Debug.Log("Combat started!");
    }

    // Function to find the nearest enemy (you can customize this logic)
    private GameObject FindNearestEnemy()
    {
        float nearestDistance = float.MaxValue;
        GameObject nearestEnemy = null;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject currentEnemy in enemies)
        {
            // Ensure only active enemies are considered
            if (!currentEnemy.activeSelf) continue;

            float distanceToEnemy = Vector3.Distance(player.transform.position, currentEnemy.transform.position);
            if (distanceToEnemy < nearestDistance)
            {
                nearestEnemy = currentEnemy;
                nearestDistance = distanceToEnemy;
            }
        }

        return nearestEnemy;
    }

    void OnPlayerEnterBattle()
    {
        SceneTransitionManager transitionManager = FindObjectOfType<SceneTransitionManager>();
        transitionManager.StartBattleTransition();
    }

    // Call this when the player flees from battle
    public void FleeFromBattle()
    {
        if (!isInCombat) return;  // Ensure the player is in combat before fleeing

        Debug.Log("Player fled the battle.");

        // Restore player's position in the dungeon before the battle
        player.transform.position = playerPositionBeforeBattle;

        // Re-enable player movement and controls
        playerControl.enabled = true;

        // Switch UI back to dungeon UI
        battleUI.SetActive(false);
        dungeonUI.SetActive(true);

        // Restore camera follow behavior
        cameraFollow.isInCombat = false;

        // Reset combat state
        isInCombat = false;

        Debug.Log("Player returned to the dungeon after fleeing.");
    }


    // Call this function when the combat is over to return to the dungeon
    public void EndCombat(bool playerWon)
    {
        if (!isInCombat) return; // Prevent ending combat if not in combat


        // Handle the case where the player wins
        if (playerWon)
        {
            // Your existing logic for deactivating the enemy
            enemy.SetActive(false);
        }

        // Reset enemy (if needed)
        enemy.SetActive(false); // Disable the enemy after defeat

        // Re-enable camera follow
        cameraFollow.isInCombat = false; // Resume camera follow

        // Disable the battle UI and bring back the dungeon elements
        battleUI.SetActive(false);
        
        dungeonUI.SetActive(true);

        // Restore the player's position
        player.transform.position = playerPositionBeforeBattle;

        // Re-enable the player's movement
        playerControl.enabled = true;

        isInCombat = false; // Reset combat state to allow future battles

        Debug.Log("Combat ended, back to dungeon!");

        // Delay to allow UI to update before re-enabling the trigger
        StartCoroutine(ResetTriggerAfterDelay(1f)); // 1 second delay
    }

    // Coroutine to reset the enemy trigger
    private IEnumerator ResetTriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnemyTrigger enemyTrigger = FindObjectOfType<EnemyTrigger>();
        if (enemyTrigger != null)
        {
            enemyTrigger.ResetTrigger(); // Re-enable enemy trigger
        }
    }


}