using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public QuestionsData QData; 

    public GameObject MultipleChoice;
    public GameObject InputAndSubmit;
    public GameObject Player;
    
    // Handling the currentHearts of the player
    private int currentHearts = 3;
    private GameObject[] hearts;

    private int currentQuestionIndex;
    private int questionCount = 0;
    private int score = 0;
    private string difficulty;
    private QuestionsData temporaryQuestionsData;

    void Start()
    {
        QData = Resources.Load<QuestionsData>("CSV DATA/QuestionData");

        Player = GameObject.FindWithTag("Player");

        hearts = GameObject.FindGameObjectsWithTag("heart");

        difficulty = GameSettings.selectedDifficulty;

        CreateTemporaryQuestionsList(difficulty.ToLower());

        if (temporaryQuestionsData != null)
        {
            SetQuestion();
        }
        else
        {
            Debug.LogError("Temporary questions list contains no questions.");
        }
    }

    void SetQuestion()
    {
        if (difficulty.ToLower().Contains("easy") || difficulty.ToLower().Contains("medium"))
        {
            InputAndSubmit.SetActive(true);
            MultipleChoice.SetActive(false);

            questionCount = temporaryQuestionsData.simpleQuestions.Count;

            currentQuestionIndex = Random.Range(0, questionCount);

            var SQuestion = temporaryQuestionsData.simpleQuestions[currentQuestionIndex] as QuestionsData.QuestionSimple;

            if (SQuestion != null)
            {
                questionText.text = SQuestion.questionText;

                submitButton.onClick.RemoveAllListeners();

                submitButton.onClick.AddListener(() =>
                {
                    CheckAnswer();
                });
            }
            else
            {
                Debug.LogError("The question is not of type QuestionSimple.");
            }
        }
        else if (difficulty.ToLower().Contains("hard"))
        {
            InputAndSubmit.SetActive(false);
            MultipleChoice.SetActive(true);

            questionCount = temporaryQuestionsData.multipleChoiceQuestions.Count;

            currentQuestionIndex = Random.Range(0, questionCount);

            var MQuestion = temporaryQuestionsData.multipleChoiceQuestions[currentQuestionIndex] as QuestionsData.QuestionMultipleChoice;

            if (MQuestion != null)
            {
                questionText.text = MQuestion.questionText;

                foreach (Button button in choicesButtons)
                {
                    button.onClick.RemoveAllListeners();
                }

                for (int i = 0; i < choicesButtons.Length; i++)
                {
                    choicesButtons[i].gameObject.SetActive(i < MQuestion.choices.Length);
                    choicesButtons[i].GetComponentInChildren<Text>().text = MQuestion.choices[i];

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
        else
        {
            Debug.LogError("Invalid difficulty selected.");
        }
    }

    void CheckAnswer()
    {
        var question = temporaryQuestionsData.simpleQuestions[currentQuestionIndex] as QuestionsData.QuestionSimple;

        if (question != null && inputText.text.ToLower() == question.correctAnswer.ToLower())
        {
            score++;
            scoreText.text = "Score: " + score;
        }

        submitButton.interactable = false;

        temporaryQuestionsData.simpleQuestions.RemoveAt(currentQuestionIndex); // Move to the if statement

        Player.GetComponent<PlayerMovement>().Move();
        
        // Decrement the current hearts and destroy the UI (heart)
        currentHearts--;
        Destroy(hearts[hearts.Length - 1]);
        hearts = hearts.Take(hearts.Count() - 1).ToArray();

        if (currentHearts == 0) {
            foreach (Button button in choicesButtons)
            {
                button.interactable = false;
            }
        }

        StartCoroutine(Next());
    }

    void CheckAnswer(int choiceIndex)
    {
        var question = temporaryQuestionsData.multipleChoiceQuestions[currentQuestionIndex] as QuestionsData.QuestionMultipleChoice;

        if (question != null && choiceIndex == question.correctChoiceIndex)
        {
            score++;
            scoreText.text = "Score: " + score;
        }

        foreach (Button button in choicesButtons)
        {
            button.interactable = false;
        }

        temporaryQuestionsData.multipleChoiceQuestions.RemoveAt(currentQuestionIndex); // Move to the if statement

        Player.GetComponent<PlayerMovement>().Move();
        
        // Decrement the current hearts and destroy the UI (heart)
        currentHearts--;
        Destroy(hearts[hearts.Length - 1]);
        hearts = hearts.Take(hearts.Count() - 1).ToArray();

        if (currentHearts == 0) {
            foreach (Button button in choicesButtons)
            {
                button.interactable = false;
            }
        }

        StartCoroutine(Next());
    }

    IEnumerator Next()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log(temporaryQuestionsData.multipleChoiceQuestions.Count);
        Debug.Log(temporaryQuestionsData.simpleQuestions.Count);

        questionCount--;

        currentQuestionIndex = Random.Range(0, questionCount);
        
        Reset();
    }

    void CreateTemporaryQuestionsList(string difficulty)
    {
        // Initialize a new instance of QuestionsData for the temporary list
        temporaryQuestionsData = ScriptableObject.CreateInstance<QuestionsData>();

        // Filter and add multiple choice questions based on difficulty
        foreach (var mcQuestion in QData.multipleChoiceQuestions)
        {
            if (mcQuestion.difficulty.ToLower() == difficulty)
            {
                temporaryQuestionsData.multipleChoiceQuestions.Add(mcQuestion);
            }
        }

        // Filter and add simple questions based on difficulty
        foreach (var simpleQuestion in QData.simpleQuestions)
        {
            if (simpleQuestion.difficulty.ToLower() == difficulty)
            {
                temporaryQuestionsData.simpleQuestions.Add(simpleQuestion);
            }
        }

        Debug.Log("Temporary questions list created with difficulty: " + difficulty);
    }

    void Reset()
    {
        foreach (Button button in choicesButtons)
        {
            button.interactable = true;
        }

        submitButton.interactable = true;

        SetQuestion();
    }
}
