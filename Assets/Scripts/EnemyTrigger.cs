using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyTrigger : MonoBehaviour
{
    

    private BattleTrigger battleTrigger; // Reference to the BattleTrigger script

    private void Start()
    {
        // Find the BattleTrigger component in the scene
        battleTrigger = FindObjectOfType<BattleTrigger>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider that entered the trigger belongs to the player
        if (other.CompareTag("Player"))
        {
            if (battleTrigger != null)
            {
                battleTrigger.StartCombat(); // Notify BattleTrigger to start combat
            }
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(battleScene);
            //DestroyPlayerOnSceneLoad();
        }
    }*/

    /*void DestroyPlayerOnSceneLoad()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }*/

}
