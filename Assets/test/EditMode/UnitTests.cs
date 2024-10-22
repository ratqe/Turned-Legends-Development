using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTests
{
    private GameObject unitObject;
    private Unit unit;

    [SetUp]
    public void Setup()
    {
        // Create a new GameObject and add the Unit component to it
        unitObject = new GameObject();
        unit = unitObject.AddComponent<Unit>();

        // Initialize basic properties of the unit
        unit.getHealth = 100;
        unit.decrementHealth = 50;  // Current health is half
        unit.healingRate = 20;      // Healing rate is 20
    }

    [Test]
    public void IncreasesHealthCorrectly()
    {
        // Act
        unit.Heal();

        // Assert
        Assert.AreEqual(70, unit.decrementHealth, "Unit's health after healing should be 70.");
    }

    [Test]
    public void ReducesHealthCorrectly()
    {
        // Arrange
        int initialHealth = unit.decrementHealth;
        int damage = 30;

        // Act
        bool isDead = unit.TakeDamage(damage);

        // Assert
        Assert.AreEqual(initialHealth - damage, unit.decrementHealth, "Unit's health after taking damage should be correct.");
        Assert.IsFalse(isDead, "Unit should not be dead after taking this amount of damage.");
    }

    [Test]
    public void LevelsUpWhenEnoughXP()
    {
        // Arrange
        unit.currentExperience = 90; // Close to leveling up
        unit.maxExperience = 100;
        int experienceGain = 20;     // Gain enough XP to level up

        // Act
        unit.GainExperience(experienceGain);

        // Assert
        Assert.AreEqual(1, unit.unitLevel, "Unit should level up to level 1.");
        Assert.AreEqual(0, unit.currentExperience, "Unit's experience should reset after leveling up.");
    }
}
