using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public Transform pfItemWorld;
    

    public Sprite healthPotionSprite;
    public Sprite manaPotionSprite;
    public Sprite speedPotionSprite;
    public Sprite weaknessPotionSprite;
    public Sprite coinSprite;
}
