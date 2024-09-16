using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadShopScene : MonoBehaviour
{
    public string sceneName = "Shop"; // Name of the shop scene

    // Level move zoned enter, if collider is a player
    // Move game to the shop scene
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Trigger Entered");

        if(other.tag == "Player")
        {
            // Player entered, so move to the shop scene
            print("Switching Scene to " + sceneName);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}