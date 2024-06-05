using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "New QuestionData", menuName = "QuestionsData")] // For Creating the Scriptable Object from the Menu

// namespace QuestionsData
// {
    public class QuestionsData : ScriptableObject
    {
        [System.Serializable]
        public struct Question
        {
            public string questionText; //For Storing the Question
            public string difficulty; //For Storing the Difficulty
            public string[] choices; //For Storing the Choices
            public string correctAnswer; //For Storing the Correct Answer
            public int correctChoiceIndex; //For Storing the index in Multiple Choice
        }
        public List<Question> Questions;
    }

// }
