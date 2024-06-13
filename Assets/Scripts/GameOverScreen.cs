using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text scoreText;
    public void SetScore(int score)
    {
        scoreText.text = "FINAL SCORE: " + score.ToString();
    }

    public void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartMenuScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
