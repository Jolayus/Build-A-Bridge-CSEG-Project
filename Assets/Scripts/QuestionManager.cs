using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public Text questionText;
    public Text scoreText;
    public Text finalScoreText;
    public InputField inputText;
    public Button submitButton;
    public Button[] choicesButtons;
    public QuestionsData QData; // For the reference of Scriptable Object

    public GameObject Player;

    private int currentQuestionIndex = 0;
    private int score = 0;

    void Start()
    {
      // Retrieve the selected difficulty from GameSettings
      string difficulty = GameSettings.selectedDifficulty;

      // Use the difficulty in your game logic
      Debug.Log("Selected Difficulty: " + difficulty);

      Player = GameObject.FindWithTag("Player");
      QData = Resources.Load<QuestionsData>("CSV DATA/QuestionData");

      if (QData != null && (QData.multipleChoiceQuestions.Count > 0 || QData.simpleQuestions.Count > 0))
      {
          SetQuestion(currentQuestionIndex);
      }
      else
      {
          Debug.LogError("QuestionsData is not assigned or contains no questions.");
      }
    }

    void SetQuestion(int CQIndex)
    {
        var question = QData.multipleChoiceQuestions[CQIndex] as QuestionsData.QuestionMultipleChoice;

        if (question != null)
        {
            questionText.text = question.questionText;

            foreach (Button button in choicesButtons)
            {
                button.onClick.RemoveAllListeners();
            }

            for (int i = 0; i < choicesButtons.Length; i++)
            {
                choicesButtons[i].gameObject.SetActive(i < question.choices.Length);
                choicesButtons[i].GetComponentInChildren<Text>().text = question.choices[i];

                int choiceIndex = i;
                choicesButtons[i].onClick.AddListener(() =>
                {
                    CheckAnswer(choiceIndex);
                });
            }
        }
        else
        {
            Debug.LogError("The question is not of type QuestionMultipleChoice.");
        }
    }

    void CheckAnswer(int choiceIndex)
    {
        var question = QData.multipleChoiceQuestions[currentQuestionIndex] as QuestionsData.QuestionMultipleChoice;

        if (question != null && choiceIndex == question.correctChoiceIndex)
        {
            score++;
            scoreText.text = "Score: " + score;
        }

        foreach (Button button in choicesButtons)
        {
            button.interactable = false;
        }

        Player.GetComponent<PlayerMovement>().Move();

        StartCoroutine(Next());
    }

    IEnumerator Next()
    {
        yield return new WaitForSeconds(1f);

        currentQuestionIndex++;

        if (currentQuestionIndex < QData.multipleChoiceQuestions.Count)
        {
            Reset();
        }
        else
        {
            float scorePercent = (float)score / QData.multipleChoiceQuestions.Count * 100;
            finalScoreText.text = $"Score: {scorePercent:F0}%";

            if (scorePercent < 50)
                finalScoreText.text += "\nStudy better next time!";
            else if (scorePercent < 60)
                finalScoreText.text += "\nGood effort!";
            else if (scorePercent < 70)
                finalScoreText.text += "\nWell done!";
            else if (scorePercent < 80)
                finalScoreText.text += "\nGreat job!";
            else
                finalScoreText.text += "\nExcellent!";
        }
    }

    public void Reset()
    {
        foreach (Button button in choicesButtons)
        {
            button.interactable = true;
        }

        SetQuestion(currentQuestionIndex);
    }
}
