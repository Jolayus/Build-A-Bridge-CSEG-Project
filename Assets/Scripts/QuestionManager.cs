using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    public TMP_Text levelText;
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
    public GameObject gameOverUI;
    public GameObject levelCompleteUI;

    public GameOverScreen gameOverScreen;
    public LevelComplete levelComplete;
    
    // Handling the currentHearts of the player
    private int currentHearts = 3;
    private GameObject[] hearts;

    private int currentQuestionIndex;
    private int questionCount = 0;
    private int questionsAnswered = 0;
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

        Dictionary<string, string> levelsDict = new Dictionary<string, string>();
        levelsDict.Add("easy", "EASY");
        levelsDict.Add("medium", "MEDIUM");
        levelsDict.Add("hard", "HARD");

        levelText.SetText(levelsDict[difficulty.ToLower()]);


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

                List<int> indices = Enumerable.Range(0, MQuestion.choices.Length).ToList();
                indices = indices.OrderBy(x => Random.value).ToList();

                for (int i = 0; i < choicesButtons.Length; i++)
                {
                    choicesButtons[i].gameObject.SetActive(i < MQuestion.choices.Length);

                    int shuffledIndex = indices[i];
                    choicesButtons[i].GetComponentInChildren<Text>().text = MQuestion.choices[shuffledIndex];

                    choicesButtons[i].onClick.AddListener(() =>
                    {
                        CheckAnswer(shuffledIndex);
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
            Player.GetComponent<PlayerMovement>().Move();
            SoundManager.Instance.PlayCorrectSound();
        } else
        {
            // Decrement the current hearts and destroy the UI (heart)
            currentHearts--;
            Destroy(hearts[hearts.Length - 1]);
            hearts = hearts.Take(hearts.Count() - 1).ToArray();
            SoundManager.Instance.PlayIncorrectSound();
        }

        inputText.text = "";

        submitButton.interactable = false;

        temporaryQuestionsData.simpleQuestions.RemoveAt(currentQuestionIndex); // Move to the if statement
        

        if (currentHearts == 0) {
            foreach (Button button in choicesButtons)
            {
                button.interactable = false;
                GameOver();
            }
        }

        if (score == 11)
        {
            NextLevel();
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
            Player.GetComponent<PlayerMovement>().Move();
            SoundManager.Instance.PlayCorrectSound();
        }
        else
        {
            // Decrement the current hearts and destroy the UI (heart)
            currentHearts--;
            Destroy(hearts[hearts.Length - 1]);
            hearts = hearts.Take(hearts.Count() - 1).ToArray();
            SoundManager.Instance.PlayIncorrectSound();
        }

        foreach (Button button in choicesButtons)
        {
            button.interactable = false;
        }

        inputText.text = "";

        temporaryQuestionsData.multipleChoiceQuestions.RemoveAt(currentQuestionIndex); // Move to the if statement

        if (currentHearts == 0) {
            foreach (Button button in choicesButtons)
            {
                button.interactable = false;
                GameOver();
            }
        }
        if (score == 11)
        {
            NextLevel();
        }
        StartCoroutine(Next());
    }

    IEnumerator Next()
    {
        yield return new WaitForSeconds(1f);

        // Debug.Log(temporaryQuestionsData.multipleChoiceQuestions.Count);
        // Debug.Log(temporaryQuestionsData.simpleQuestions.Count);

        questionCount--;
        questionsAnswered++;

        if (questionsAnswered < 10)
        {
            currentQuestionIndex = Random.Range(0, questionCount);

            Reset();
        }
        else
        {
            GameSettings.completedLevels.Add(difficulty.ToLower());

            // Take the player back to the start menu to select a new difficulty
            SceneManager.LoadScene("StartMenuScene");
        }
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

    void GameOver()
    {
        gameOverScreen.SetScore(score);
        gameOverUI.SetActive(true);
        
    }
    
    void NextLevel()
    {
        GameSettings.completedLevels.Add(difficulty.ToLower());
        levelComplete.SetScore(score);
        levelCompleteUI.SetActive(true);
    }
}
