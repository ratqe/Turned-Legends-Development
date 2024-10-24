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

    public AudioClip newSong;
    public Button speedUpButton;
    public Button detailsButton;  // Reference to the Details button
    public GameObject popupPanel; // Reference to the Panel acting as a popup
    public TextMeshProUGUI popupText;  // The Text component in the popup panel (if using TextMeshPro)

    public BattleState state;

    private Animator anim;
    private Animator playerAnim;
    public Text playerDamageText;
    public Text enemyDamageText;

    private int attackCount = 0;  // Counter to track the number of attacks
    private int defendCount = 0;
    private int enemyAttackCount = 0;

    private bool hasAttacked = false;  // Flag to track if the player has attacked 
    private bool isSpeedUp = false;
    private bool combatStarted = false;
    private bool buttonAction = false;

    private Vector3 playerSpawnPosition;
    //[SerializeField]
    //private string battleScene = "Battle 1";
    public int sceneBuildIndex;


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
        speedUpButton.onClick.RemoveAllListeners();
        state = BattleState.START;
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.ChangeSong(newSong);  // Play the new song in this specific scene
        }
        hasAttacked = false;  // Reset the flag at the start of the battle
        speedUpButton.onClick.AddListener(ToggleSpeed);
        detailsButton.onClick.AddListener(ShowDetailsPopup);
    }

    public void StartBattleFromTrigger()
    {
        if (!combatStarted)
        {
            combatStarted = true;
            StartCoroutine(SetupBattle());
        }
    }

    public void ShowDetailsPopup()
    {
        popupText.text = "Hello";  // Set the text in the popup
        popupPanel.SetActive(true); // Show the popup panel
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false); // Hide the popup panel
    }



    private bool isButtonPressed = false;  // Flag to prevent multiple triggers

    public void ToggleSpeed()
    {
        if (isButtonPressed)
        {
            return;  
        }
        
        isButtonPressed = true;  

        Debug.Log("ToggleSpeed() called.");

        if (isSpeedUp)
        {
            Time.timeScale = 1f;
            speedUpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Speed Up";
            Debug.Log("Speed set to normal.");
        }
        else
        {
            Time.timeScale = 5f;
            speedUpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Normal Speed";
            Debug.Log("Speed set to fast.");
        }

        isSpeedUp = !isSpeedUp;

        // Re-enable button after a short delay
        StartCoroutine(ResetButtonPress());
    }

    private IEnumerator ResetButtonPress()
    {
        yield return new WaitForSeconds(0.2f);  
        isButtonPressed = false;  // Re-enable the button press
    }


    IEnumerator SetupBattle()
    {
        // Get player and enemy units directly from their respective battle stations
        if (playerUnit == null)
        {
            // Find the Unit component on the PlayerBattleStation
            playerUnit = playerBattleStation.GetComponent<Unit>();
            playerAnim = playerBattleStation.GetComponent<Animator>();

            // Store the original player position for future reference
            playerSpawnPosition = playerBattleStation.position;
        }
        else
        {
            // Reset player's position and other stats if needed
            playerUnit.transform.position = playerSpawnPosition;
            playerUnit.ResetForNewBattle();  // Custom method to reset health/stats
        }

        if (enemyUnit == null)
        {
            // Similar approach for the enemy battle station
            enemyUnit = enemyBattleStation.GetComponent<Unit>();
            anim = enemyBattleStation.GetComponent<Animator>();
        }
        else
        {
            enemyUnit.transform.position = enemyBattleStation.position;
            enemyUnit.ResetForNewBattle();  // Custom method to reset health/stats
        }

        dialogueText.text = "An enemy " + enemyUnit.unitName + " approaches!";
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        enemyAttackCount = 0;
        attackCount = 0;
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

        enemyBattleStation.rotation = Quaternion.identity;

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
        enemyBattleStation.rotation = Quaternion.identity;
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

        // Move the enemy to a special attack position
        Vector3 originalPosition = enemyBattleStation.position;
        Vector3 specialAttackPosition = playerBattleStation.position + new Vector3(2f, 0, 0);  // Enemy gets closer

        float moveDuration = 0.7f;
        float elapsedTime = 0f;

        // Move the enemy closer for the special attack
        while (elapsedTime < moveDuration)
        {
            enemyBattleStation.position = Vector3.Lerp(originalPosition, specialAttackPosition, (elapsedTime / moveDuration));
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

        float finalDamage = damage;
        if (playerUnit.isDefending)
        {
            finalDamage = (int)(damage * (1 - playerUnit.defensePercentage));
        }
        playerDamageText.text = "-" + finalDamage.ToString() + " HP";

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

    IEnumerator EnemySpecialAttack()
    {
        dialogueText.text = enemyUnit.unitName + " is charging a powerful attack!";
        int specialDamage = enemyUnit.specialDamage;

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
        anim.SetTrigger("Enemy1Attack");

        yield return new WaitForSeconds(1f);

        // Calculate final damage base on the player set defense
        float finalDamage = specialDamage;
        if (playerUnit.isDefending)
        {
            finalDamage = (int)(specialDamage * (1 - playerUnit.defensePercentage));
        }
        
        // Apply special attack damage to the player
        bool isDead = playerUnit.TakeDamage((int)finalDamage);
        playerHUD.SetHP(playerUnit.decrementHealth);

        dialogueText.text = "The enemy deals a massive blow!";

        playerDamageText.text = "-" + finalDamage.ToString() + " HP";
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

        int specialDamage = playerUnit.specialDamage;

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

        // Apply special attack damage to the enemy
        enemyDamageText.text = "-" + specialDamage.ToString() + " HP";
        bool isDead = enemyUnit.TakeDamage(specialDamage);
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
        playerUnit.Heal();

        playerHUD.SetHP(playerUnit.decrementHealth);
        dialogueText.text = "Regenerate! Heal for " + playerUnit.healingRate;

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
        if (defendCount < 1)
        {
            dialogueText.text = "Defend once first before healing!";
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

        // Move only 1 unit straight backward
        Vector3 defendPosition = originalPosition - new Vector3(1.0f, 0, 0);  // Move 1 unit backward

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
        SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);

        // Reference to the BattleTrigger script to handle the flee action
        BattleTrigger battleTrigger = FindObjectOfType<BattleTrigger>();
        if (battleTrigger != null)
        {
            battleTrigger.FleeFromBattle();  // Call the FleeFromBattle method
        }

        // Additional cleanup if needed (like stopping the battle music)
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.RevertToOriginalSong();
        }
    }

    public void ResetBattleSystem()
    {
        combatStarted = false; 
        state = BattleState.START; 
        hasAttacked = false; 
    }


    IEnumerator EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle! Congrats!";
            PlayerWins();
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated :/";
            PlayerLoses();
        }

        yield return new WaitForSeconds(3f);

        Time.timeScale = 1f;
        isSpeedUp = false;
        
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.RevertToOriginalSong();
        }

        // Reset the battle system state
        BattleSystem battleSystem = FindObjectOfType<BattleSystem>();
        if (battleSystem != null)
        {
            battleSystem.ResetBattleSystem();
        }

        // Reset the battle trigger
        BattleTrigger battleTrigger = FindObjectOfType<BattleTrigger>();
        if (battleTrigger != null)
        {
            battleTrigger.EndCombat(true); //reset the player position and other UI elements
        }

        // Return player to original position (before battle started)
        PlayerControl playerControl = FindObjectOfType<PlayerControl>();
        if (playerControl != null)
        {
            playerControl.transform.position = battleTrigger.playerPositionBeforeBattle; // Restore saved position
        }

        // Reset the enemy trigger to allow for new encounters after a short delay
        yield return new WaitForSeconds(1f); 
        EnemyTrigger enemyTrigger = FindObjectOfType<EnemyTrigger>();
        if (enemyTrigger != null)
        {
            enemyTrigger.ResetTrigger(); // Re-enable enemy trigger
        }
    }

    private void PlayerWins()
    {
        // Get the experience value from the enemy unit
        int experienceGained = enemyUnit.experienceValue;

        // Add experience to the player's unit
        playerUnit.GainExperience(experienceGained);

        //ExperienceManager.Instance.AddExperience(experienceGained);

        // Display the message in the dialogue box
        dialogueText.text = "You won the battle! Gained " + experienceGained + " XP!";

        // You can add any other logic here, such as rewards, transitioning, etc.
    }

    private void PlayerLoses()
    {
        dialogueText.text = "You lost the battle...";

        Time.timeScale = 1f;
        isSpeedUp = false;

        // Load the Lobby scene
        SceneManager.LoadScene(6);

        // Add any logic for what happens when the player loses (e.g., game over)
    }







}