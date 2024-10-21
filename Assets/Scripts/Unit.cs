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
}

