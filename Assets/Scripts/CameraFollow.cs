using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float followSpeed = 2f;
    public bool isInCombat = false;  // Boolean to check if in battle
    public Vector3 battleCameraPosition; // Fixed position for the battle

    private Vector3 targetPosition;

    public void Update()
    {
        //if (!isInCombat)  // Disable camera follow during combat
        //{
        //    Vector3 targetPosition = player.position + offset;
        //    transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        //}
        if (isInCombat)
        {
            // In battle, set camera to a fixed position
            targetPosition = battleCameraPosition;
        }
        else
        {
            // Follow the player during exploration
            targetPosition = player.position + offset;
        }

        // Smooth transition to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    // Call this when battle starts
    public void EnterBattle()
    {
        isInCombat = true;
        // Set the camera to a fixed position for the battle
        targetPosition = battleCameraPosition;
    }

    // Call this when battle ends
    public void ExitBattle()
    {
        isInCombat = false;
        // Camera will go back to following the player
    }
}

