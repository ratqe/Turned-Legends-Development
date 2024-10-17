using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public static GameObject selectedPlayer; 

    public void SelectPlayer1()
    {
        selectedPlayer = player1Prefab;
        StartBattle();
    }

    public void SelectPlayer2()
    {
        selectedPlayer = player2Prefab;
        StartBattle();
    }

    private void StartBattle()
    {
        
        SceneManager.LoadScene(0); 
    }
}
