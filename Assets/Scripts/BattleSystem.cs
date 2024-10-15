using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Import TextMeshPro for UI

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
    public TextMeshProUGUI tipText;  // UI Text element for random tips using TextMeshPro
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    public Button speedUpButton;
    public AudioClip newSong;



    public BattleState state;

    private Animator anim;
    private Animator playerAnim;
    public Text playerDamageText;
    public Text enemyDamageText;
    private bool hasAttacked = false;  // Flag to track if the player has attacked 
    private int attackCount = 0;  // Counter to track the number of attacks
    private int defendCount = 0;
    private int enemyAttackCount = 0;
    private bool isSpeedUp = false;
    

    private bool buttonAction = false;

    private Vector3 playerSpawnPosition;
    [SerializeField]
    private string battleScene = "Battle 1";

    public GameObject endBattlePanel;

    // Array of random gameplay tips
    private string[] tips = {

        "Tips: Remember to heal when you're low on health!",
        "Tips: Defending reduces incoming damage significantly.",
        "Tips: Use strong attacks to finish off weakened enemies.",
        "Tips: Switch up your tactics to outsmart your enemies!",
        "Tips: Pay attention to enemy attack patterns!"
    };

    public float elapsedTime { get; private set; }

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.ChangeSong(newSong);  // Play the new song in this specific scene
        }
        hasAttacked = false;  // Reset the flag at the start of the battle
        speedUpButton.onClick.AddListener(ToggleSpeed);
    }

    public void ToggleSpeed()
    {
        

        if (!isSpeedUp)
        {
            Time.timeScale = 5f;
            speedUpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Normal Speed";  // Update button text
        }
        else
        {
            Time.timeScale = 1f;
            speedUpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Speed Up";  // Update button text
        }

        isSpeedUp = !isSpeedUp;  // Toggle the speed flag
    }


    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        anim = enemyGO.GetComponent<Animator>();
        playerAnim = playerGO.GetComponent<Animator>();
        // player original position 
        playerSpawnPosition = playerBattleStation.position;
        dialogueText.text = "An enemy " + enemyUnit.unitName + " approaches!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose action:";
        buttonAction = false; //reset button boolean
    }

    // Method to display a random tip on the screen
    void DisplayRandomTip()
    {
        int randomIndex = Random.Range(0, tips.Length);  // Pick a random tip index
        tipText.text = tips[randomIndex];  // Display the selected tip in the UI
    }

    IEnumerator PlayerAttack()
    {
        DisplayRandomTip();  // Show a random tip when attacking

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
        // Rest of the attack sequence...

        // Restore enemy to original rotation
        elapsedTime = 0f;
        while (elapsedTime < fallDuration)
        {
            enemyBattleStation.rotation = Quaternion.Slerp(fallRotation, originalRotation, (elapsedTime / fallDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move the player back to the original position
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            playerBattleStation.position = Vector3.Lerp(attackPosition, originalPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        // Hide damage after a short delay
        enemyDamageText.text = "";

        // Rest of the attack sequence...

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
        dialogueText.text = enemyUnit.unitName + " is preparing an attack!";
        enemyAttackCount++;  // Increment the enemy's attack counter

        if (enemyAttackCount >= 4)
        {
            // Trigger the enemy's special move on the fourth attack
            StartCoroutine(EnemySpecialAttack());
            enemyAttackCount = 0;  // Reset the counter after the special attack
            yield break;  // Exit this coroutine after the special attack
        }

        float damage = enemyUnit.damage;  // Enemy's base damage

        // Move the enemy straight toward the player for the attack
        Vector3 originalPosition = enemyBattleStation.position;
        Vector3 attackPosition = playerBattleStation.position + new Vector3(1.5f, 0, 0);  // Adjust the X value as needed

        float attackMoveDuration = 0.9f;  // Duration for the movement toward the player
        float elapsedTime = 0f;

        // Smoothly move the enemy toward the player
        while (elapsedTime < attackMoveDuration)
        {
            enemyBattleStation.position = Vector3.Lerp(originalPosition, attackPosition, (elapsedTime / attackMoveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Trigger attack animation for the enemy
        anim.SetTrigger("Enemy1Attack");

        // Let the attack animation play
        yield return new WaitForSeconds(0.6f);

        // Apply damage to the player
        bool isDead = playerUnit.TakeDamage((int)damage);
        playerHUD.SetHP(playerUnit.decrementHealth);  // Update the player's HUD

        playerDamageText.text = "-" + damage.ToString() + " HP";
        yield return new WaitForSeconds(1f);
        playerDamageText.text = "";

        // Move the enemy back to its original position after the attack
        float returnMoveDuration = 0.5f;
        elapsedTime = 0f;
        while (elapsedTime < returnMoveDuration)
        {
            enemyBattleStation.position = Vector3.Lerp(attackPosition, originalPosition, (elapsedTime / returnMoveDuration));
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


    IEnumerator EnemySpecialAttack()
    {
        dialogueText.text = enemyUnit.unitName + " is charging a powerful attack!";
        int specialDamage = enemyUnit.damage * 3;  // Stronger special attack damage

        // Move the enemy to a special attack position
        Vector3 originalPosition = enemyBattleStation.position;
        Vector3 specialAttackPosition = playerBattleStation.position + new Vector3(1f, 0, 0);  // Enemy gets closer

        float moveDuration = 0.7f;
        float elapsedTime = 0f;

        // Move the enemy closer for the special attack
        while (elapsedTime < moveDuration)
        {
            enemyBattleStation.position = Vector3.Lerp(originalPosition, specialAttackPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Trigger the special attack animation
        anim.SetTrigger("EnemySpecialAttack");

        yield return new WaitForSeconds(1f);

        // Apply special attack damage to the player
        bool isDead = playerUnit.TakeDamage(specialDamage);
        playerHUD.SetHP(playerUnit.decrementHealth);

        dialogueText.text = "The enemy deals a massive blow!";

        playerDamageText.text = "-" + specialDamage.ToString() + " HP";
        yield return new WaitForSeconds(1f);
        playerDamageText.text = "";

        // Move the enemy back after the special attack
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            enemyBattleStation.position = Vector3.Lerp(specialAttackPosition, originalPosition, (elapsedTime / moveDuration));
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






    IEnumerator PlayerSpecialAttack()
    {
        DisplayRandomTip();  // Show a random tip when the special attack starts

        int damage = playerUnit.damage * 3;  // Stronger final blow

        Vector3 originalPosition = playerBattleStation.position;

        // Define waypoints for the player to move around before attacking
        Vector3[] waypoints = new Vector3[]
        {
        playerBattleStation.position + new Vector3(3f, 0, 0),  // Right
        playerBattleStation.position + new Vector3(3f, 2f, 0),  // Up
        playerBattleStation.position + new Vector3(-3f, 2f, 0),  // Left
        playerBattleStation.position + new Vector3(-3f, 0, 0)   // Down
        };

        float moveDuration = 0.5f;

        // Move the player around the waypoints
        foreach (Vector3 waypoint in waypoints)
        {
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                playerBattleStation.position = Vector3.Lerp(originalPosition, waypoint, (elapsedTime / moveDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            originalPosition = waypoint;
        }

        // Move the player toward the enemy for the final blow
        Vector3 attackPosition = enemyBattleStation.position - new Vector3(3f, 0, 0);

        float attackMoveDuration = 1f;
        float elapsedAttackTime = 0f;

        while (elapsedAttackTime < attackMoveDuration)
        {
            playerBattleStation.position = Vector3.Lerp(originalPosition, attackPosition, (elapsedAttackTime / attackMoveDuration));
            elapsedAttackTime += Time.deltaTime;
            yield return null;
        }

        // Trigger attack animation for the player
        playerAnim.SetTrigger("Player1Attack");

        yield return new WaitForSeconds(1f);

        // Deal the final blow to the enemy
        enemyDamageText.text = "-" + damage.ToString() + " HP";
        bool isDead = enemyUnit.TakeDamage(damage);
        enemyHUD.SetHP(enemyUnit.decrementHealth);
        dialogueText.text = "A devastating blow!";

        // Trigger fall animation or effect for the enemy
        Quaternion originalRotation = enemyBattleStation.rotation;
        Quaternion fallRotation = Quaternion.Euler(0f, 0f, 90f);  // Rotate 90 degrees to simulate fall

        float fallDuration = 0.5f;
        float elapsedTimeFall = 0f;

        // Smoothly rotate the enemy to simulate falling
        while (elapsedTimeFall < fallDuration)
        {
            enemyBattleStation.rotation = Quaternion.Slerp(originalRotation, fallRotation, (elapsedTimeFall / fallDuration));
            elapsedTimeFall += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);  // Pause for a moment to let the enemy stay on the ground

        // Rest of the special attack sequence...

        // Restore enemy to original rotation (optional, if you want to make the enemy stand up again)
        elapsedTimeFall = 0f;
        while (elapsedTimeFall < fallDuration)
        {
            enemyBattleStation.rotation = Quaternion.Slerp(fallRotation, originalRotation, (elapsedTimeFall / fallDuration));
            elapsedTimeFall += Time.deltaTime;
            yield return null;
        }

        // Move the player back to the original spawn position after the attack
        elapsedAttackTime = 0f;
        while (elapsedAttackTime < attackMoveDuration)
        {
            playerBattleStation.position = Vector3.Lerp(attackPosition, playerSpawnPosition, (elapsedAttackTime / attackMoveDuration));
            elapsedAttackTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        // Hide damage after a short delay
        enemyDamageText.text = "";

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


    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(24);

        playerHUD.SetHP(playerUnit.decrementHealth);
        dialogueText.text = "Regenerate! Heal for 24 HP!";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }



    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN || buttonAction)
            return;

        attackCount++;  // Increment the attack counter

        if (attackCount >= 3)
        {
            // Trigger the special move after 3 attacks
            StartCoroutine(PlayerSpecialAttack());
            attackCount = 0;  // Reset the counter after the special attack
        }
        else
        {
            hasAttacked = true;  // Set this flag when the player attacks
            StartCoroutine(PlayerAttack());
        }
        buttonAction = true; //user has used a button
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN || buttonAction)
        {
            return;
        }

        // will check if the player has defended at least twice
        if (defendCount < 2)
        {
            dialogueText.text = "Defend twice first before healing!";
            return;
        }

        // if defended at least twice, allow healing
        StartCoroutine(PlayerHeal());

        // Reset the defend count after healing
        defendCount = 0;
        buttonAction = true;
    }


    public void OnFleeButton()
    {
        if (state != BattleState.PLAYERTURN || buttonAction)
            return;

        if (hasAttacked)
        {
            dialogueText.text = "Can't flee after attacking";
            return;  // Prevent the player from fleeing if they've attacked
        }

        dialogueText.text = "You fled yippe!";
        StartCoroutine(FleeBattle());

        buttonAction = true;
    }


    public void OnDefendButton()
    {
        if (state != BattleState.PLAYERTURN || buttonAction)
            return;

        StartCoroutine(PlayerDefend());
        DisplayRandomTip();  // Show a random tip when defending
        buttonAction = true; //user has used a button
    }

    IEnumerator PlayerDefend()
    {
        Vector3 originalPosition = playerBattleStation.position;
        Vector3 defendPosition = originalPosition - new Vector3(1.0f, 0, 0);  // Move player 1 unit backward

        float elapsedTime = 0f;
        float moveDuration = 0.4f;  // Duration for the movement
        dialogueText.text = "Player is defending!";
        // Smoothly move the player backward
        while (elapsedTime < moveDuration)
        {
            playerBattleStation.position = Vector3.Lerp(originalPosition, defendPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        playerUnit.isDefending = true;  // Defense is activated here

        defendCount++;
        yield return new WaitForSeconds(2f);
        // End the player's turn and switch to enemy turn
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());

        // Wait for the enemy turn to finish before moving back
        while (state == BattleState.ENEMYTURN)
        {
            yield return null;
        }

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
        SceneManager.LoadScene(battleScene);

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
            dialogueText.text = "You won the battle congrats!!!!";
            // Displaying options for continuing the battle or exiting
            EndBattleOptions();
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated :/";
            yield return new WaitForSeconds(3f);

            SceneManager.LoadScene(battleScene);
            MusicManager musicManager = FindObjectOfType<MusicManager>();
            if (musicManager != null)
            {
                musicManager.RevertToOriginalSong();
            }
        }


    }

    IEnumerator SetupNewBattle()
    {
        Destroy(enemyBattleStation.GetChild(0).gameObject); // Remove the defeated enemy

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        anim = enemyGO.GetComponent<Animator>();

        dialogueText.text = "A new enemy " + enemyUnit.unitName + " appears!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }



    void EndBattleOptions()
    {
        endBattlePanel.SetActive(true); // Show the panel when the battle ends
    }

    public void ContinueButton()
    {
        endBattlePanel.SetActive(false); // Hide the panel
        StartCoroutine(SetupNewBattle()); // starts a new battle 
    }

    public void ExitButton()
    {
        SceneManager.LoadScene(6); // Load back to lobby
    }



}