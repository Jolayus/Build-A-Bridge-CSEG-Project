using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelsScript : MonoBehaviour
{
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    
    void Start()
    {
        easyButton.onClick.AddListener(() => OnDifficultyButtonClicked("Easy"));
        mediumButton.onClick.AddListener(() => OnDifficultyButtonClicked("Medium"));
        hardButton.onClick.AddListener(() => OnDifficultyButtonClicked("Hard"));
    }

    void OnDifficultyButtonClicked(string difficulty)
    {
        // Store the selected difficulty
        GameSettings.selectedDifficulty = difficulty;

        // Load the main game scene
        SceneManager.LoadScene("MainScene");
    }
}
