using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public Button[] difficultyButtons;

    void Start()
    {
        DifficultyEnabled();
    }

    public void DifficultyEnabled()
    {
        foreach (Button button in difficultyButtons)
        {
            string difficulty = button.GetComponentInChildren<Text>().text.ToLower();
            if (GameSettings.completedLevels.Contains(difficulty) || difficulty == "easy")
            {
                button.interactable = true;
            }
            else if (difficulty == "medium" && GameSettings.completedLevels.Contains("easy"))
            {
                button.interactable = true;
            }
            else if (difficulty == "hard" && GameSettings.completedLevels.Contains("medium"))
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
