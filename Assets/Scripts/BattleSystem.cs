using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;




    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public Text dialogueText;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public AudioClip newSong;

    public BattleState state;

    private Animator anim;
    private Animator playerAnim;
    public Text playerDamageText;
    public Text enemyDamageText;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.ChangeSong(newSong);  // Play the new song in this specific scene
        }
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        anim = enemyGO.GetComponent<Animator>();
        playerAnim = playerGO.GetComponent<Animator>();

        dialogueText.text = "A wild " + enemyUnit.unitName + " approaches fr";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        yield return new WaitForSeconds(2f);


        state = BattleState.PLAYERTURN;
        PlayerTurn();


    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose action:";
    }

    IEnumerator PlayerAttack()
    {
        int damage = playerUnit.damage;  // player damage

        // Move the player closer to the enemy before attacking
        Vector3 originalPosition = playerBattleStation.position;
        Vector3 attackPosition = enemyBattleStation.position - new Vector3(1f, 0, 0);  // Move player 1 unit in front of the enemy

        float elapsedTime = 0f;
        float moveDuration = 0.4f;  // Duration for the movement toward the enemy

        // Smoothly move the player toward the enemy
        while (elapsedTime < moveDuration)
        {
            playerBattleStation.position = Vector3.Lerp(originalPosition, attackPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Triggering animation
        playerAnim.SetTrigger("Player1Attack");

        // Let the animation play first, then apply damage
        yield return new WaitForSeconds(1f);

        // Damage dealt by the player
        enemyDamageText.text = "-" + damage.ToString() + " HP";

        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.SetHP(enemyUnit.decrementHealth);
        dialogueText.text = "The attack is successful!";

        yield return new WaitForSeconds(2f);

        // Hide damage after a short delay
        enemyDamageText.text = "";

        // Move the player back to the original position
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            playerBattleStation.position = Vector3.Lerp(attackPosition, originalPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Check if the enemy is dead
        if (isDead)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        int damage = enemyUnit.damage;

        anim.SetTrigger("Enemy1Attack");
        dialogueText.text = enemyUnit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

        playerDamageText.text = "-" + damage.ToString() + " HP";

        // Make the player take a step back when attacked
        Vector3 originalPosition = playerBattleStation.position;
        Vector3 stepBackPosition = originalPosition + new Vector3(-1f, 0, 0);  // Step back 1 unit on the x-axis

        float elapsedTime = 0f;
        float moveDuration = 0.5f;  // Duration for the step back movement

        // Smoothly move the player back
        while (elapsedTime < moveDuration)
        {
            playerBattleStation.position = Vector3.Lerp(originalPosition, stepBackPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.decrementHealth);

        yield return new WaitForSeconds(1f);

        // Return the player to the original position
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            playerBattleStation.position = Vector3.Lerp(stepBackPosition, originalPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerDamageText.text = "";

        if (isDead)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }


    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnFleeButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        dialogueText.text = "You fled yippe";
        StartCoroutine(FleeBattle());
    }



    IEnumerator FleeBattle()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Lobby");

        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.RevertToOriginalSong();
        }
    }

    IEnumerator EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle congrats!!!! gjg";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated :/";
        }

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Lobby");
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.RevertToOriginalSong();
        }

    }


}





