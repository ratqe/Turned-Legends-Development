using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab; // Assign your player prefab in the inspector

    private static GameObject playerInstance;

    private void Start()
    {
        // Find the dungeon generator in the scene
        RoomFirstDungeonGenerator dungeonGenerator = FindObjectOfType<RoomFirstDungeonGenerator>();

        if (dungeonGenerator != null)
        {
            // Check if a player already exists in the scene
            if (playerInstance == null)
            {
                SpawnPlayer(dungeonGenerator.spawnPosition);
            }
        }
    }

    private void SpawnPlayer(Vector2Int position)
    {
        if (playerPrefab != null)
        {
            // Instantiate the player at the spawn position
            playerInstance = Instantiate(playerPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        }
    }
}