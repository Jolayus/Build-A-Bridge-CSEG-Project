using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public Text questionText;
    public Text scoreText;
    public Text finalScoreText;
    public Button[] choicesButtons;
    public QuestionsData QData; // For the reference of Scriptable Object

    public GameObject Player;

    private int currentQuestionIndex = 0;
    private static int score = 0;


    // Start is called before the first frame update
    void Start()
    {
        SetQuestion(currentQuestionIndex);
        Player = GameObject.FindWithTag("Player");
    }

    void SetQuestion(int CQIndex)
    {
        questionText.text = QData.Questions[CQIndex].questionText;

        foreach (Button button in choicesButtons){
            button.onClick.RemoveAllListeners();
        }

        for (int i = 0; i < choicesButtons.Length; i++)
        {
            choicesButtons[i].GetComponentInChildren<Text>().text = QData.Questions[CQIndex].choices[i];
            
            int choiceIndex = i;

            choicesButtons[i].onClick.AddListener(() => 
            {
                CheckAnswer(choiceIndex);
            });
        }
    }

    void CheckAnswer(int choiceIndex){
        if (choiceIndex == QData.Questions[currentQuestionIndex].correctChoiceIndex)
        {
            score++;
            scoreText.text = "" + score;

            foreach (Button button in choicesButtons)
            {
                button.interactable = false;
            }

            Player.GetComponent<PlayerMovement>().Move();

            // Next Question
            StartCoroutine(Next());
        }
        else
        {
            foreach (Button button in choicesButtons)
            {
                button.interactable = false;
            }

            // Next Question
            StartCoroutine(Next());
        }
    }

    IEnumerator Next()
    {
        yield return new WaitForSeconds(1f);

        currentQuestionIndex++;

        if (currentQuestionIndex < QData.Questions.Count)
        {   
            //Reset the UI and enable the buttons
            Reset();
        }
        else
        {   
            //Calculating the score percentage
            float scorePercent = (float)score / (float)QData.Questions.Count * 100;

            finalScoreText.text = "Score: " + scorePercent.ToString("F0") + "%";

            //Appropriate message for final score
            if (scorePercent < 50){
                finalScoreText.text += "\nStudy better next time!";
            }
            else if (scorePercent < 60)
            {
                finalScoreText.text += "\nStudy better next time!";
            }
            else if (scorePercent < 70)
            {
                finalScoreText.text += "\nStudy better next time!";
            }
            else if (scorePercent < 80)
            {
                finalScoreText.text += "\nStudy better next time!";
            }
            else
            {
                finalScoreText.text += "\nStudy better next time!";
            }
        }
    }

    public void Reset(){
        foreach (Button button in choicesButtons)
        {
            button.interactable = true;
        }

        SetQuestion(currentQuestionIndex);
    }
}
