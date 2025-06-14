using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDropHandler
{
    public Image playerImage = null;
    public Image mirrorImage = null;
    public Image healthNumberImage = null;
    public Image glowImage = null;

    public int maxHealth;
    public int health; // current health
    public int mana;

    public bool isPlayer;
    public bool isFire; // Whether the enemy is a fire monster or not.

    public AudioSource dealAudio = null;
    public AudioSource healAudio = null;
    public AudioSource mirrorAudio = null;
    public AudioSource smashAudio = null;

    public GameObject[] manaBalls = new GameObject[5];

    private Animator animator = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateHealth();
        UpdateManaBalls();
    }

    internal void PlayHitAnimation()
    {
        if(animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        if (!GameController.instance.isPlayable) return;

        GameObject obj = eventData.pointerDrag;
        if(obj != null)
        {
            Card card = obj.GetComponent<Card>();
            if (card != null)
            {
                GameController.instance.UseCard(card, this, GameController.instance.playerHand);
            }
            else Debug.LogError("Missing Card component.");
        }
    }

    internal void UpdateHealth()
    {
        if(health >= 0 && health < GameController.instance.healthNumbers.Length)
        {
            healthNumberImage.sprite = GameController.instance.healthNumbers[health];
        } else
        {
            Debug.Log("Invalid health value: " + health);
        }
    }

    internal void SetMirror(bool on)
    {
        mirrorImage.gameObject.SetActive(on);
    }

    internal bool HasMirrorEffect()
    {
        return mirrorImage.gameObject.activeSelf;
    }

    internal void UpdateManaBalls()
    {
        for(int i = 0; i < 5; i++)
        {
            if (mana > i) manaBalls[i].SetActive(true);
            else manaBalls[i].SetActive(false);
        }
    }

    internal void PlayMirrorSound()
    {
        mirrorAudio.Play();
    }
    internal void PlaySmashSound()
    {
        smashAudio.Play();
    }
    internal void PlayHealSound()
    {
        healAudio.Play();
    }
    internal void PlayDealSound()
    {
        dealAudio.Play();
    }
}
