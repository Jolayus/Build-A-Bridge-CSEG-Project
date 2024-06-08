using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New QuestionData", menuName = "QuestionsData")] // For manually creating the Scriptable Object from the Menu

public class QuestionsData : ScriptableObject
{
    public List<Question> Questions = new List<Question>();
    
    [System.Serializable]
    public struct Question
    {
        public string questionText; //For Storing the Question
        public string difficulty; //For Storing the Difficulty
        public string[] choices; //For Storing the Choices
        public string correctAnswer; //For Storing the Correct Answer
        public int correctChoiceIndex; //For Storing the index in Multiple Choice
    }
}