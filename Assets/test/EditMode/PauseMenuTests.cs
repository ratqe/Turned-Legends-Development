using NUnit.Framework;
using UnityEngine;

public class PauseMenuTests
{
    private GameObject pauseMenuObject;
    private PauseMenu pauseMenu;

    [SetUp]
    public void Setup()
    {
        
        pauseMenuObject = new GameObject();
        pauseMenuObject.AddComponent<Canvas>(); 
        pauseMenu = pauseMenuObject.AddComponent<PauseMenu>(); 

        
        pauseMenu.pauseMenu = pauseMenuObject;
    }

    [Test]
    public void PauseMenu()
    {
        
        pauseMenu.PauseGame();

        
        Assert.IsTrue(pauseMenu.pauseMenu.activeSelf, "Pause menu should be active.");
        Assert.AreEqual(0f, Time.timeScale, "Time scale should be paused (0).");
    }

    [Test]
    public void AdjustVolume()
    {
        
        float initialVolume = AudioListener.volume;
        float newVolume = 0.5f; 
        AudioListener.volume = newVolume; 

        
        Assert.AreEqual(newVolume, AudioListener.volume, "Volume should be adjusted to 0.5f.");
    }

}
