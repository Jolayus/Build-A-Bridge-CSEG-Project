using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text completedLevel;
    public void SetScore(int score)
    {
        scoreText.text = "FINAL SCORE: " + score.ToString();
    }

    public void NextLevel()
    {
        if(completedLevel.ToString().ToLower() == "easy")
        {
            GameSettings.selectedDifficulty = "medium";
            GameSettings.completedLevels.Add("easy");
        }else if (completedLevel.ToString().ToLower() == "medium")
        {
            GameSettings.selectedDifficulty = "hard";
            GameSettings.completedLevels.Add("medium");
        }
        
        SceneManager.LoadScene("StartMenuScene");
    }
}
