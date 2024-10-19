using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public GameObject dungeonUI; // All the UI elements for dungeon exploration
    public GameObject battleUI;  // The battle UI to show

    private void Start()
    {
        battleUI.SetActive(false);  // Make sure battle UI is hidden initially
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCombat();
        }
    }

    private void StartCombat()
    {
        // Disable dungeon elements
        dungeonUI.SetActive(false);

        // Enable battle elements
        battleUI.SetActive(true);

        // Stop player movement or any other dungeon-related activities
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerControl>().enabled = false;

        // Start the battle logic here
    }

    public void EndCombat()
    {
        // When combat is over, reverse the process
        battleUI.SetActive(false);
        dungeonUI.SetActive(true);

        // Re-enable player movement
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerControl>().enabled = true;
    }
}

