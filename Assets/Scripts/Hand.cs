using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hand
{
    public Card[] cards = new Card[3];
    public Transform[] cardPositions = new Transform[3];
    public string[] animationNames = new string[3];
    public bool isPlayerHand;

    public void RemoveCard(Card card)
    {
        for(int i = 0; i < 3; i++)
        {
            if (cards[i] == card)
            {
                GameObject.Destroy(cards[i].gameObject);
                cards[i] = null;

                if(isPlayerHand) GameController.instance.playerDeck.DealCard(this);
                else GameController.instance.playerDeck.DealCard(this);
                break;
            }
        }
    }

    internal void ClearHand()
    {
        for(int i = 0; i < 3; i++)
        {
            GameObject.Destroy(cards[i].gameObject);
            cards[i] = null;
        }
    }

    public bool ContainsCard(Card card)
    {
        for(int i = 0; i < 3; i++)
        {
            if (cards[i] == card) return true;
        }
        return false;
    }
}
