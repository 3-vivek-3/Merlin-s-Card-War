using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BurnZone : MonoBehaviour, IDropHandler
{

    public AudioSource burnAudio = null;
    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerDrag;
        Card card = obj.GetComponent<Card>();

        if(card != null)
        {
            PlayBurnSound();
            GameController.instance.playerHand.RemoveCard(card);
            GameController.instance.NextPlayerTurn();
        }

    }

    internal void PlayBurnSound()
    {
        burnAudio.Play();
    }
}
