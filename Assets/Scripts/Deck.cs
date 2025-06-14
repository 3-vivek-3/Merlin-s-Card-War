using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class Deck
{
    public List<CardData> cardDatas = new List<CardData>();

    public void Create()
    {
        // 1.) create a list of CardData for the deck
        // 2.) randomize the order of the deck.

        List<CardData> cardDataInOrder = new List<CardData>();

        foreach(CardData cardData in GameController.instance.cards)
        {
            for(int i = 0; i < cardData.numberInDeck; i++)
            {
                cardDataInOrder.Add(cardData);
            }
        }
        while(cardDataInOrder.Count > 0)
        {
            int randomIndex = Random.Range(0, cardDataInOrder.Count);
            cardDatas.Add(cardDataInOrder[randomIndex]);
            cardDataInOrder.RemoveAt(randomIndex);
        }
    }

    private CardData GetRandomCardData()
    {
        CardData result = null;
        if (cardDatas.Count == 0) Create();

        result = cardDatas[0];
        cardDatas.RemoveAt(0);

        return result;
    }

    private Card CreateNewCard(Vector3 position, string animationName)
    {
        GameObject newCard = GameObject.Instantiate(GameController.instance.cardPrefab, GameController.instance.canvas.gameObject.transform);

        newCard.transform.position = position;

        Card card = newCard.GetComponent<Card>();

        if (card)
        {
            card.cardData = GetRandomCardData();
            card.Initialize();

            // In the code, its is CardGraphics that has the Animator component, which is a child.
            Animator animator = newCard.GetComponentInChildren<Animator>();

            if (animator) {
                animator.CrossFade(animationName, 0);

            }
            else Debug.LogError("No animator found");

            return card;
        }
        else
        {
            Debug.LogError("No card component found.");
            return null;
        }
    }
    internal void DealCard(Hand hand)
    {
        for (int i = 0; i < 3; i++)
        {
            if(hand.cards[i] == null)
            {
                if (hand.isPlayerHand) GameController.instance.player.PlayDealSound();
                else GameController.instance.enemy.PlayDealSound();

                hand.cards[i] = CreateNewCard(hand.cardPositions[i].position, hand.animationNames[i]);
                return;
            }
        }
    }
}
