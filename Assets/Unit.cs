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



    public bool TakeDamage(int dmg)
    {
        decrementHealth -= dmg;

        if (decrementHealth <= 0)
            return true;
        else
            return false;
    }

}
