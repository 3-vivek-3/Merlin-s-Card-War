using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI scoreText = null;
    public TextMeshProUGUI enemiesKilledText = null;

    private void Awake()
    {
        scoreText.text = "Score: " + GameController.instance.playerScore.ToString();
        enemiesKilledText.text = "Enemies killed: " + GameController.instance.enemiesKilled.ToString();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
