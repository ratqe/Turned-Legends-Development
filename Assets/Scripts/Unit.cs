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

    public bool isDefending = false;  // Add this flag

    public bool TakeDamage(int dmg)
    {
        if (isDefending)
        {
            // If the unit is defending, reduce the damage by a specific amount
            dmg = Mathf.Max(0, dmg - 2);  // Damage reduced by 2
        }

        decrementHealth -= dmg;

        if (decrementHealth <= 0)
            return true;
        else
            return false;
    }
}

