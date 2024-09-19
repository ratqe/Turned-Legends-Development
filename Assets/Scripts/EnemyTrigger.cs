using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyTrigger : MonoBehaviour
{
    [SerializeField]
    private string battleScene = "Battle";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(battleScene);
            DestroyPlayerOnSceneLoad();
        }
    }

    void DestroyPlayerOnSceneLoad()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }

}
