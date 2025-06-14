using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    public Sprite[] healthNumbers = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];

    // List of all possible card types.
    public List<CardData> cards = new List<CardData>();

    public Player player = null;
    public Player enemy = null;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck = new Deck();

    public Hand playerHand = new Hand();
    public Hand enemyHand = new Hand();

    public Canvas canvas = null;
    public GameObject cardPrefab = null;
    public GameObject effectFromLeftPrefab = null;
    public GameObject effectFromRightPrefab = null;

    public Sprite fireballEffect = null;
    public Sprite iceballEffect = null;
    public Sprite multiFireballEffect = null;
    public Sprite multiIceballEffect = null;
    public Sprite icefireballEffect = null;
    public Sprite destroyEffect = null;

    public bool isPlayable = false;
    public bool playerTurn = true;

    public TextMeshProUGUI turnText = null;
    public TextMeshProUGUI scoreText = null;

    public int playerScore = 0;
    public int enemiesKilled = 0;

    public Image enemySkipTurnImage = null;

    public Sprite fireEnemySprite = null;
    public Sprite iceEnemySprite = null;

    public AudioSource playerDieAudio = null;
    public AudioSource enemyDieAudio = null;


    public void Awake()
    {
        instance = this;

        SetNewEnemy();
        playerDeck.Create();
        enemyDeck.Create();
        StartCoroutine(DealHands());
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
    public void SkipTurn()
    {
        if (playerTurn && isPlayable) NextPlayerTurn();
    }

    internal IEnumerator DealHands()
    {
        isPlayable = false;

        yield return new WaitForSeconds(1);
        for (int i = 0; i < 3; i++)
        {
            playerDeck.DealCard(playerHand);
            enemyDeck.DealCard(enemyHand);
            yield return new WaitForSeconds(1);
        }
        isPlayable = true;
    }

    internal bool UseCard(Card cardPlayed, Player targetPlayer, Hand fromHand)
    {
        // check if the card is allowed to be used. 
        if (!CardPlayValid(cardPlayed, targetPlayer, fromHand)) return false;

        isPlayable = false;

        // if it is, then play card.
        CastCard(cardPlayed, targetPlayer, fromHand);

        GameController.instance.player.glowImage.gameObject.SetActive(false);
        GameController.instance.enemy.glowImage.gameObject.SetActive(false);



        fromHand.RemoveCard(cardPlayed);

        return false;

    }

    // not only checks the card validity, but also the validity of the play.
    internal bool CardPlayValid(Card cardPlayed, Player targetPlayer, Hand fromHand)
    {
        bool valid = false;
        if (cardPlayed == null) return false;

        if(fromHand.isPlayerHand) // the player hand
        {
            if(cardPlayed.cardData.cost <= player.mana)
            {
                if (targetPlayer.isPlayer && cardPlayed.cardData.isDefenseCard) valid = true;
                if (!targetPlayer.isPlayer && !cardPlayed.cardData.isDefenseCard) valid = true;
            }
        }
        else // the enemy hand
        {
            if (cardPlayed.cardData.cost <= enemy.mana)
            {
                if (!targetPlayer.isPlayer && cardPlayed.cardData.isDefenseCard) valid = true;
                if (targetPlayer.isPlayer && !cardPlayed.cardData.isDefenseCard) valid = true;
            }
        }
        return valid;
    }

    internal void CastCard(Card cardPlayed, Player targetPlayer, Hand fromHand)
    {

        if(cardPlayed.cardData.isMirrorCard) // mirror card
        {
            targetPlayer.SetMirror(true);
            NextPlayerTurn();
            targetPlayer.PlayMirrorSound();
            isPlayable = true;

        } else
        {
            if(cardPlayed.cardData.isDefenseCard) // health cards
            {
                StartCoroutine(CastHealEffect(cardPlayed, targetPlayer));
                if (fromHand.isPlayerHand) playerScore += cardPlayed.cardData.damage * 10;
            } 
            else // attack cards
            {
                CastAttackEffect(cardPlayed, targetPlayer);
                // score update is done in Effect.cs, due to mirror considerations.
            }
            UpdateScore();

        }

        if (fromHand.isPlayerHand)
        {
            GameController.instance.player.mana -= cardPlayed.cardData.cost;
            GameController.instance.player.UpdateManaBalls();
        }
        else 
        {
            GameController.instance.enemy.mana -= cardPlayed.cardData.cost;
            GameController.instance.enemy.UpdateManaBalls();
        }
    }

    private IEnumerator CastHealEffect(Card cardPlayed, Player targetPlayer)
    {
        // update health values
        targetPlayer.health += cardPlayed.cardData.damage;
        if (targetPlayer.health > targetPlayer.maxHealth) targetPlayer.health = targetPlayer.maxHealth;
        UpdateHealths();

        targetPlayer.PlayHealSound();

        yield return new WaitForSeconds(0.5f);

        NextPlayerTurn();
        isPlayable = true;
    }

    internal void CastAttackEffect(Card cardPlayed, Player targetPlayer)
    {
        if (effectFromRightPrefab == null || effectFromLeftPrefab == null) Debug.LogError("Prefabs not assigned.");

        // instantiate effect prefab
        GameObject effectObj;
        if (targetPlayer.isPlayer) effectObj = Instantiate(effectFromRightPrefab, canvas.gameObject.transform);
        else effectObj = Instantiate(effectFromLeftPrefab, canvas.gameObject.transform);
        
        Effect effect = effectObj.GetComponent<Effect>();

        if (effect != null)
        {
            // set effect component parameters
            effect.targetPlayer = targetPlayer;
            effect.sourceCard = cardPlayed;
            switch (cardPlayed.cardData.type)
            {
                case CardData.DamageType.Fire:
                    if (cardPlayed.cardData.isMultiAttackCard) effect.effectImage.sprite = multiFireballEffect;
                    else effect.effectImage.sprite = fireballEffect;
                    effect.PlayFireSound();
                    break;
                case CardData.DamageType.Ice:
                    if (cardPlayed.cardData.isMultiAttackCard) effect.effectImage.sprite = multiIceballEffect;
                    else effect.effectImage.sprite = iceballEffect;
                    effect.PlayIceSound();
                    break;
                case CardData.DamageType.Both:
                    effect.effectImage.sprite = icefireballEffect;
                    effect.PlayFireSound();
                    effect.PlayIceSound();
                    break;
                case CardData.DamageType.Destruct:
                    effect.effectImage.sprite = destroyEffect;
                    effect.PlayDestroySound();
                    break;
            }
        }
        else Debug.LogError("Effect component missing.");
    }

    internal void UpdateHealths()
    {
        player.UpdateHealth();
        enemy.UpdateHealth();

        if(player.health <= 0)
        {
            // game over
            StartCoroutine(GameOver());
        }

        if(enemy.health <= 0)
        {
            // increase kill count
            enemiesKilled++;
            playerScore += 100;
            UpdateScore();
            // new enemy
            StartCoroutine(NewEnemy());
        }
    }

    internal void NextPlayerTurn()
    {
        playerTurn = !playerTurn;

        if (!playerTurn) isPlayable = false;
        else isPlayable = true;

        bool enemyIsDead = false;

        if(playerTurn)
        {
            if (player.mana < 5) player.mana++;
        }
        else
        {
            if (enemy.health > 0)
            {
                if (enemy.mana < 5) enemy.mana++;
            }
            else enemyIsDead = true;
        }

        if (enemyIsDead)
        {
            playerTurn = !playerTurn;
            if (player.mana < 5) player.mana++;
        }
        else
        {
            if (playerTurn) turnText.text = "Merlin's turn";
            else turnText.text = "Enemy turn";

            if (!playerTurn)
            {
                EnemyTurn();
            }
        }

        player.UpdateManaBalls();
        enemy.UpdateManaBalls();
    }

    private IEnumerator NewEnemy()
    {
        enemy.gameObject.SetActive(false);

        // clear enemy hand
        enemyHand.ClearHand();

        yield return new WaitForSeconds(1);

        // set up a new enemy
        SetNewEnemy();

        // show new enemy
        enemy.gameObject.SetActive(true);

        // deal new enemy hand
        StartCoroutine(DealHands());
    }// wait 1 second

    private void SetNewEnemy()
    {
        enemy.mana = 0;
        enemy.health = 5;
        enemy.UpdateHealth();
        enemy.isFire = true;
        if(UnityEngine.Random.Range(0, 2) == 0) enemy.isFire = false;

        if (enemy.isFire) enemy.playerImage.sprite = fireEnemySprite;
        else enemy.playerImage.sprite = iceEnemySprite;
    }

    private void EnemyTurn()
    {
        Card card = AIChooseCard();
        StartCoroutine(EnemyCastCard(card));

    }

    private Card AIChooseCard()
    {
        List<Card> availableCards = new List<Card>();
        for (int i = 0; i < 3; i++)
        {
            if (CardPlayValid(enemyHand.cards[i], enemy, enemyHand)) availableCards.Add(enemyHand.cards[i]);
            else if (CardPlayValid(enemyHand.cards[i], player, enemyHand)) availableCards.Add(enemyHand.cards[i]);
        }

        if (availableCards.Count == 0) // no card that can be played.
        {
            NextPlayerTurn();
            return null;
        }

        int choice = UnityEngine.Random.Range(0, availableCards.Count);
        return availableCards[choice];
    }

    private IEnumerator EnemyCastCard(Card cardPlayed)
    {
        yield return new WaitForSeconds(1);

        if(cardPlayed)
        {
            // flip enemy card
            FlipEnemyCard(cardPlayed);
            
            yield return new WaitForSeconds(2);

            // play enemy card
            if (cardPlayed.cardData.isDefenseCard) UseCard(cardPlayed, enemy, enemyHand);
            else UseCard(cardPlayed, player, enemyHand);

            yield return new WaitForSeconds(2);

            // deal replacement card
            enemyDeck.DealCard(enemyHand);

            yield return new WaitForSeconds(1);
        }
        else
        {
            // show enemy skip turn
            enemySkipTurnImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            // hide enemy skip turn
            enemySkipTurnImage.gameObject.SetActive(false);
        }
    }

    private void FlipEnemyCard(Card cardPlayed)
    {
        Animator animator = cardPlayed.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Flip");
        }
        else Debug.LogError("No card animator found.");
    }

    internal void UpdateScore()
    {
        scoreText.text = "Enemies killed: " + enemiesKilled.ToString() + "\nScore: " + playerScore.ToString();
    }

    internal void PlayPlayerDieSound()
    {
        playerDieAudio.Play();
    }
    internal void PlayEnemyDieSound()
    {
        enemyDieAudio.Play();
    }

}
