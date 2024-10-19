using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    public GameObject dungeonUI; // Reference to your Dungeon UI GameObject
    public GameObject battleUI;  // Reference to your Battle UI GameObject

    private Vector3 playerPositionBeforeBattle;
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
        battleUI.SetActive(false);  // Battle UI should be hidden initially
    }



    // This function starts combat by switching UI elements
    public void StartCombat()
    {
        // Save player's position before battle
        playerPositionBeforeBattle = player.transform.position;

        if (isInCombat) return; // Prevent starting combat if already in combat

        isInCombat = true; // Set combat state to true

        // Other battle initialization code
        cameraFollow.isInCombat = true;  // Stop following the player

        // Disable dungeon UI elements (e.g., map, player controls)
        dungeonUI.SetActive(false);

        // Enable the battle UI
        battleUI.SetActive(true);

        // Disable the player's movement during combat
        playerControl.enabled = false;



        // Optionally, start battle logic here (e.g., turn-based system)
        Debug.Log("Combat started!");
    }

    void OnPlayerEnterBattle()
    {
        SceneTransitionManager transitionManager = FindObjectOfType<SceneTransitionManager>();
        transitionManager.StartBattleTransition();
    }



    // Call this function when the combat is over to return to the dungeon
    public void EndCombat()
    {
        if (!isInCombat) return; // Prevent ending combat if not in combat

        // Reset player and enemy positions (if necessary)
        enemy.SetActive(false);  // Disable the enemy after defeat

        // Re-enable camera follow
        cameraFollow.isInCombat = false;  // Resume camera follow

        // Disable the battle UI and bring back the dungeon elements
        battleUI.SetActive(false);
        dungeonUI.SetActive(true);

        // Restore the player's position
        player.transform.position = playerPositionBeforeBattle;

        // Re-enable the player's movement
        playerControl.enabled = true;

        // Ending the combat 
        EndCombat();

        Debug.Log("Combat ended, back to dungeon!");
    }
}