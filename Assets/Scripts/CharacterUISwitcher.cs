using UnityEngine;
using UnityEngine.UI;

public class CharacterUISwitcher : MonoBehaviour
{
    public GameObject[] characterUI; // Array of UI elements for each character
    private int currentIndex = 0; // To track the current character being displayed

    public Button nextButton;
    public Button previousButton;

    void Start()
    {
        UpdateCharacterUI();
        previousButton.interactable = false; // Initially, disable previous button
    }

    public void ShowNextCharacter()
    {
        if (currentIndex < characterUI.Length - 1)
        {
            characterUI[currentIndex].SetActive(false); // Hide current character UI
            currentIndex++; // Move to next character
            characterUI[currentIndex].SetActive(true); // Show next character UI
            UpdateButtons();
        }
    }

    public void ShowPreviousCharacter()
    {
        if (currentIndex > 0)
        {
            characterUI[currentIndex].SetActive(false); // Hide current character UI
            currentIndex--; // Move to previous character
            characterUI[currentIndex].SetActive(true); // Show previous character UI
            UpdateButtons();
        }
    }

    private void UpdateCharacterUI()
    {
        // Ensure only the current character's UI is active
        for (int i = 0; i < characterUI.Length; i++)
        {
            characterUI[i].SetActive(i == currentIndex);
        }
    }

    private void UpdateButtons()
    {
        // Enable/disable next/previous buttons based on the current index
        nextButton.interactable = currentIndex < characterUI.Length - 1;
        previousButton.interactable = currentIndex > 0;
    }
}
