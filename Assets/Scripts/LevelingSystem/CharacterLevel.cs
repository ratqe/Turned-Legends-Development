using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLevel : MonoBehaviour
{
    [SerializeField]
    int decrementHealth, getHealth, // decrementHealth = currentHealth, getHealth = maxHealth, easier way to read it
        currentExperience, maxExperience,
        currentLevel;

    private void OnEnable()
    {
        ExperienceManager.Instance.OnExperienceChange += HandleExperienceChange;
    }

    private void OnDisable()
    {
        ExperienceManager.Instance.OnExperienceChange -= HandleExperienceChange;
    }

    private void HandleExperienceChange(int newExperience)
    {
        currentExperience += newExperience;
        if(currentExperience >= maxExperience)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        getHealth += 10;
        decrementHealth = getHealth;

        currentLevel++;

        currentExperience = 0;
        maxExperience = 300;
    }
}
