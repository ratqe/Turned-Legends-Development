using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelection1 : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject player3Prefab;
    public GameObject player4Prefab;
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

    public void SelectPlayer3()
    {
        selectedPlayer = player3Prefab;
        StartBattle();
    }

    public void SelectPlayer4()
    {
        selectedPlayer = player4Prefab;
        StartBattle();
    }

    private void StartBattle()
    {
        
        SceneManager.LoadScene(12); 
    }
}
