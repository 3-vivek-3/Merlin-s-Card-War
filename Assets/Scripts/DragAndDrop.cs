using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!GameController.instance.isPlayable) return;

        Card card = GetComponent<Card>();

        if (GameController.instance.enemyHand.ContainsCard(card)) return;

        transform.position += (Vector3)eventData.delta;

        bool overPlayerCard = false;
        foreach(GameObject obj in eventData.hovered)
        {
            Player targetPlayer = obj.GetComponent<Player>();
            if (targetPlayer != null)
            {
                if (GameController.instance.CardPlayValid(card, targetPlayer, GameController.instance.playerHand))
                {
                    targetPlayer.glowImage.gameObject.SetActive(true);
                    overPlayerCard = true;
                }
            }

            BurnZone burnZone = obj.GetComponent<BurnZone>();
            if(burnZone != null) card.burnImage.gameObject.SetActive(true);
            else card.burnImage.gameObject.SetActive(false);
        }

        if(!overPlayerCard)
        {
            GameController.instance.player.glowImage.gameObject.SetActive(false);
            GameController.instance.enemy.glowImage.gameObject.SetActive(false);
        }

    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        transform.position = originalPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
