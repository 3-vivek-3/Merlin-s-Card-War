using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
    // Card data refers to CARD TYPE
    // These are not the actual card game objects. Thus, it's a scriptable object, so it can be reused.
{
    public enum DamageType
    {
        Fire,
        Ice,
        Both,
        Destruct
    }

    public string title;

    public string description;

    public int cost;

    public int damage;

    public int numberInDeck;

    public DamageType type;

    public Sprite cardImage;

    public Sprite frameImage;

    public bool isDefenseCard = false;

    public bool isMirrorCard = false;

    public bool isMultiAttackCard = false;

    public bool isDestructCard = false;
}
