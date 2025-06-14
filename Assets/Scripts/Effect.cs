using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    public Player targetPlayer = null;
    public Card sourceCard = null;
    public Image effectImage = null;

    public AudioSource iceAudio = null;
    public AudioSource fireAudio = null;
    public AudioSource destroyAudio = null;

    public void EndTrigger()
    {
        bool reflect = false;
        if(targetPlayer.HasMirrorEffect())
        {
            reflect = true;
            targetPlayer.SetMirror(false);

            targetPlayer.PlaySmashSound();

            if (targetPlayer.isPlayer) GameController.instance.CastAttackEffect(sourceCard, GameController.instance.enemy);
            else GameController.instance.CastAttackEffect(sourceCard, GameController.instance.player);

            Destroy(gameObject);
            return;
        }

        int damage = sourceCard.cardData.damage;
        if(!targetPlayer.isPlayer) // enemy
        {
            if (sourceCard.cardData.type == CardData.DamageType.Fire && targetPlayer.isFire) damage /= 2;
            if (sourceCard.cardData.type == CardData.DamageType.Ice && !targetPlayer.isFire) damage /= 2;
        }

        targetPlayer.health -= damage;
        targetPlayer.PlayHitAnimation();
        if (!targetPlayer.isPlayer) GameController.instance.playerScore += damage * 10;

        GameController.instance.UpdateScore();
        GameController.instance.UpdateHealths();

        if(targetPlayer.health <= 0)
        {
            targetPlayer.health = 0;
            if (targetPlayer.isPlayer) GameController.instance.PlayPlayerDieSound();
            else GameController.instance.PlayEnemyDieSound();
        }

        if(!reflect)GameController.instance.NextPlayerTurn();

        GameController.instance.isPlayable = true;

        Destroy(gameObject);

    }

    internal void PlayIceSound()
    {
        iceAudio.Play();
    }
    internal void PlayFireSound()
    {
        fireAudio.Play();
    }
    internal void PlayDestroySound()
    {
        destroyAudio.Play();
    }
}
