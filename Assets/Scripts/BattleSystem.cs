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
        Vector3 attackPosition = enemyBattleStation.position - new Vector3(3f, 0, 0);  // Move player 1 unit in front of the enemy

        float elapsedTime = 0f;
        float moveDuration = 1f;  // Duration for the movement toward the enemy

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

        // Apply fall effect to enemy
        Quaternion originalRotation = enemyBattleStation.rotation;
        Quaternion fallRotation = Quaternion.Euler(0f, 0f, 90f);  // Rotate 90 degrees to simulate fall

        elapsedTime = 0f;
        float fallDuration = 0.5f;  // Duration for the enemy to fall

        // Smoothly rotate the enemy to simulate falling
        while (elapsedTime < fallDuration)
        {
            enemyBattleStation.rotation = Quaternion.Slerp(originalRotation, fallRotation, (elapsedTime / fallDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);  // Pause for a moment to let the enemy stay on the ground

        // Restore enemy to original rotation
        elapsedTime = 0f;
        while (elapsedTime < fallDuration)
        {
            enemyBattleStation.rotation = Quaternion.Slerp(fallRotation, originalRotation, (elapsedTime / fallDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

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
        float damage = enemyUnit.damage;  // Enemy's base damage

        // Move the enemy closer to the player before attacking
        Vector3 originalPosition = enemyBattleStation.position;

        // Position the enemy directly in front of the player
        // Reduce the distance to a smaller value to position the enemy very close to the player
        Vector3 attackPosition = playerBattleStation.position + new Vector3(4f, 0, 0);  // Adjust the X value as needed

        float elapsedTime = 0f;
        float moveDuration = 0.9f;  // Duration for the movement toward the player

        // Smoothly move the enemy toward the player
        while (elapsedTime < moveDuration)
        {
            enemyBattleStation.position = Vector3.Lerp(originalPosition, attackPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Trigger attack animation for the enemy
        anim.SetTrigger("Enemy1Attack");

        

        // Let the attack animation play
        yield return new WaitForSeconds(0.6f);  // Adjust this duration to match your animation length

        // Apply damage to the player
        bool isDead = playerUnit.TakeDamage((int)damage);
        playerHUD.SetHP(playerUnit.decrementHealth);  // Update the player's HUD

        // Move the enemy back to its original position after the attack
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            enemyBattleStation.position = Vector3.Lerp(attackPosition, originalPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Check if the player is dead
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
    public void OnDefendButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerDefend());
    }

    IEnumerator PlayerDefend()
    {
        // Move the player backward when defending
        Vector3 originalPosition = playerBattleStation.position;
        Vector3 defendPosition = originalPosition - new Vector3(1.0f, 0, 0);  // Move player 1 unit backward

        float elapsedTime = 0f;
        float moveDuration = 0.4f;  // Duration for the movement

        // Smoothly move the player backward
        while (elapsedTime < moveDuration)
        {
            playerBattleStation.position = Vector3.Lerp(originalPosition, defendPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dialogueText.text = "Player is defending!";

        // Activate defense (reduced damage)
        playerUnit.isDefending = true;

        // Hold the defend position until the enemy turn is over
        yield return new WaitForSeconds(4f);  // Optional pause before enemy turn

        // End the player's turn and switch to enemy turn
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());

        // After the enemy's attack, return the player to the original position
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            playerBattleStation.position = Vector3.Lerp(defendPosition, originalPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Turn off defending after the enemy's attack
        playerUnit.isDefending = false;
    }





    IEnumerator FleeBattle()
    {
        // Define how far the player will dash backwards and how long it will take
        Vector3 originalPosition = playerBattleStation.position;
        Vector3 fleePosition = originalPosition - new Vector3(5f, 0, 0);  // Dash 1 unit backward

        float elapsedTime = 0f;
        float dashDuration = 0.5f;  // Duration for the dash

        // Smoothly move the player backward
        while (elapsedTime < dashDuration)
        {
            playerBattleStation.position = Vector3.Lerp(originalPosition, fleePosition, (elapsedTime / dashDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // After the dash is complete, pause briefly before switching scenes
        yield return new WaitForSeconds(1f);

        // Load the Lobby scene
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





