using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public Image transitionImage;  // Reference to the Image component (the fade panel)
    public float fadeDuration = 1f;  // Duration of the fade effect
    public GameObject battleUI;  // Reference to your Battle UI GameObject

    void Start()
    {
        // Ensure the transition starts fully transparent
        transitionImage.color = new Color(0, 0, 0, 0);
    }

    public void StartBattleTransition()
    {
        StartCoroutine(FadeOutAndIn());
    }

    private IEnumerator FadeOutAndIn()
    {
        // Fade out (black screen)
        yield return StartCoroutine(FadeTo(1.0f));

        // Now switch to the battle scene (you can either load a new scene or activate UI)
        EnterBattle();

        // Fade back in (from black screen to gameplay)
        yield return StartCoroutine(FadeTo(0.0f));
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        Color currentColor = transitionImage.color;
        float alpha = currentColor.a;

        for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            float newAlpha = Mathf.Lerp(alpha, targetAlpha, normalizedTime);
            transitionImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }

        // Ensure the final alpha is set correctly
        transitionImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }

    private void EnterBattle()
    {
        // Set the isInCombat flag and adjust camera position here
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        cameraFollow.EnterBattle();

        battleUI.SetActive(true);
    }
}

