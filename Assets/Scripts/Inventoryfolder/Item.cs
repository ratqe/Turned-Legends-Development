using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType
    {
        Coin,
        HealthPotion,
        ManaPotion,
        SpeedPotion,
        WeaknessPotion,
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.Coin:           return ItemAssets.Instance.coinSprite;
            case ItemType.HealthPotion:   return ItemAssets.Instance.healthPotionSprite;
            case ItemType.ManaPotion:     return ItemAssets.Instance.manaPotionSprite;
            case ItemType.SpeedPotion:    return ItemAssets.Instance.speedPotionSprite;
            case ItemType.WeaknessPotion: return ItemAssets.Instance.weaknessPotionSprite;
        }
    }
    
}
