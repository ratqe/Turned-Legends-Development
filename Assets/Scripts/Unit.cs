using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int damage;
    public int specialDamage;
    public int getHealth;
    public int decrementHealth;
    public float defensePercentage;
    public int healingRate;

    public int currentExperience;  // Player's current XP
    public int maxExperience;      // XP needed to level up
    public int experienceValue = 100;    // XP awarded when enemy is defeated

    public bool isDefending = false;  // Add this flag

    public bool TakeDamage(int dmg)
    {
        if (isDefending)
        {
            // If the unit is defending, reduce the damage by the set percentage%
            dmg = Mathf.Max(0, (int)(dmg * (1 - defensePercentage)));

        }

        decrementHealth -= dmg;

        if (decrementHealth <= 0)
            return true;
        else
            return false;
    }

	public void Heal()
	{
        // Healing based on unit fixed amount
		decrementHealth += healingRate;

        // Ensure health doesn't exceed maxHealth
		if (decrementHealth > getHealth)
        {
            decrementHealth = getHealth;
        }
			
	}

    // Method to gain experience
    public void GainExperience(int amount)
    {
        currentExperience += amount;

        // Check if we've gained enough experience to level up
        if (currentExperience >= maxExperience)
        {
            LevelUp();
        }
    }

    // Level-up logic
    public void LevelUp()
    {
        // Increase stats on level-up (you can adjust these increments)
        unitLevel++;
        getHealth += 10;
        damage += 2;
        specialDamage += 1;

        // Heal the unit to full on level-up
        decrementHealth = getHealth;

        // Reset experience and increase maxExperience for the next level
        currentExperience = 0;
        maxExperience += 300;  // Increase how much XP is required for the next level
    }

    public void ResetForNewBattle()
    {
        // Reset health and any other stats needed for a new battle
        decrementHealth = getHealth;
        currentExperience = 0;
        isDefending = false;
    }
}

