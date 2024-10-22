using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUITests
{
    private GameObject characterObject1;
    private GameObject characterObject2;
    private CharacterUISwitcher characterUISwitcher;

    [SetUp]
    public void Setup()
    {

        GameObject switcherObject = new GameObject();
        characterUISwitcher = switcherObject.AddComponent<CharacterUISwitcher>();

        
        characterObject1 = new GameObject("Character1");
        characterObject2 = new GameObject("Character2");

        
        characterUISwitcher.characterUI = new GameObject[] { characterObject1, characterObject2 };

        
        characterUISwitcher.nextButton = new GameObject("NextButton").AddComponent<Button>();
        characterUISwitcher.previousButton = new GameObject("PreviousButton").AddComponent<Button>();

        
        characterObject1.SetActive(true); // Show first character UI initially
        characterObject2.SetActive(false); // Hide second character UI

        
    }

    [Test]
    public void ShowsCorrectUI()
    {
        
        characterUISwitcher.ShowNextCharacter();

        
        Assert.IsTrue(characterObject2.activeSelf, "Character 2 UI should be active.");
        Assert.IsFalse(characterObject1.activeSelf, "Character 1 UI should be inactive.");
    }
}
