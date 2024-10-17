using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float followSpeed = 2f;
    public bool isInCombat = false;  // Add this flag

    void LateUpdate()
    {
        if (!isInCombat)  // Disable camera follow during combat
        {
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}

