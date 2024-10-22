using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerControlTests
{
    private GameObject playerObject;
    private PlayerControl playerControl;

    [SetUp]
    public void Setup()
    {
        // Create a new GameObject and add the PlayerControl component to it
        playerObject = new GameObject();
        playerControl = playerObject.AddComponent<PlayerControl>();
    }

    [Test]
    public void PlayerControl()
    {
        // Arrange
        float expectedMoveSpeed = 1f; // The default moveSpeed value

        // Act
        float actualMoveSpeed = playerControl.moveSpeed;

        // Assert
        Assert.AreEqual(expectedMoveSpeed, actualMoveSpeed, "The initial moveSpeed should be set to 1f.");
    }
}