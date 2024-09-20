using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int damage;
    public int getHealth;
    public int decrementHealth;
    public int maxHealth;

    public bool isDefending = false;  // Add this flag

    public bool TakeDamage(int dmg)
    {
        if (isDefending)
        {
            // If the unit is defending, reduce the damage by at least 50%
            dmg = Mathf.Max(0, (int)(dmg * 0.5f));
        Debug.Log("Player is defending. Reduced damage to: " + dmg);
        }

        decrementHealth -= dmg;

        if (decrementHealth <= 0)
            return true;
        else
            return false;
    }

	public void Heal(int amount)
	{
		decrementHealth += amount;
		if (decrementHealth > getHealth)
        {
            decrementHealth = getHealth;
        }
			
	}
}

