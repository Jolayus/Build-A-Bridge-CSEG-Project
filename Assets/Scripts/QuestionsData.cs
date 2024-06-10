using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New QuestionData", menuName = "QuestionsData")]
public class QuestionsData : ScriptableObject
{
    public List<QuestionMultipleChoice> multipleChoiceQuestions = new List<QuestionMultipleChoice>();
    public List<QuestionSimple> simpleQuestions = new List<QuestionSimple>();

    [System.Serializable]
    public abstract class BaseQuestion
    {
        public string questionText; // Common property for storing the Question
        public string difficulty; // Common property for storing the Difficulty
        public abstract QuestionType GetQuestionType(); // Abstract method to get question type
    }

    public enum QuestionType
    {
        MultipleChoice,
        Simple
    }

    [System.Serializable]
    public class QuestionMultipleChoice : BaseQuestion
    {
        public string[] choices; // For storing the choices
        public string correctAnswer; // For storing the correct answer
        public int correctChoiceIndex; // For storing the index of the correct choice

        public override QuestionType GetQuestionType()
        {
            return QuestionType.MultipleChoice;
        }
    }

    [System.Serializable]
    public class QuestionSimple : BaseQuestion
    {
        public string correctAnswer; // For storing the correct answer

        public override QuestionType GetQuestionType()
        {
            return QuestionType.Simple;
        }
    }
}
