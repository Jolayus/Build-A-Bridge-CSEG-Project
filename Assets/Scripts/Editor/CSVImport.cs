using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CSVImportMenu : MonoBehaviour
{
    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("CSV Import/Generate Question Data")]
    public static void CSVGenerate()
    {
        if (Selection.activeObject == null)
        {
            Debug.Log("Please select a CSV file");
            return;
        }

        string path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(Selection.activeObject));

        if (!IsCSVFile(path))
        {
            Debug.Log("Please select a CSV file");
            return;
        }

        List<Dictionary<string, object>> rawCSVData = CSVReading.CSVReader.Read(path);

        if(rawCSVData.Count > 0)
        {
            bool confirmed = EditorUtility.DisplayDialog("Generate Question Data", $"Are you sure you want to generate { rawCSVData.Count } question data from the selected CSV file?", "Yes", "No");

            if (confirmed) PerformGeneration(rawCSVData);
        }
        else
        {
            Debug.Log("No Entries in the selected CSV file");
            return;
        }
    }

    private static void PerformGeneration(List<Dictionary<string, object>> csvData)
    {
        List<QuestionsData.Question> questionsList = new List<QuestionsData.Question>();

        for (int i = 0; i < csvData.Count; i++)
        {
            Dictionary<string, object> _potentialQuestionEntry = csvData[i];

            string questionText = _potentialQuestionEntry["QUESTION"].ToString();
            string difficulty = _potentialQuestionEntry["DIFFICULTY"].ToString();
            string correctAnswer = _potentialQuestionEntry["CORRECT ANSWER"].ToString();

            string[] choices = new string[4];
            choices[0] = _potentialQuestionEntry["CORRECT ANSWER"].ToString();
            choices[1] = _potentialQuestionEntry["WRONG 1"].ToString();
            choices[2] = _potentialQuestionEntry["WRONG 2"].ToString();
            choices[3] = _potentialQuestionEntry["WRONG 3"].ToString();

            int correctChoiceIndex = System.Array.IndexOf(choices, correctAnswer);

            QuestionsData.Question question = new QuestionsData.Question
            {
                questionText = questionText,
                difficulty = difficulty,
                correctAnswer = correctAnswer,
                choices = choices,
                correctChoiceIndex = correctChoiceIndex
            };

            questionsList.Add(question);

            Debug.Log($"Question : {questionText}");
            Debug.Log($"Difficulty : {difficulty}");
            Debug.Log($"Correct Answer : {correctAnswer}");
            Debug.Log($"Choices : {string.Join(", ", choices)}");
        }

        CreateScriptableObjectQuestionData(questionsList);
    }

    private static void CreateScriptableObjectQuestionData(List<QuestionsData.Question> questionsList)
    {
        QuestionsData newQuestionEntry = ScriptableObject.CreateInstance<QuestionsData>();
        newQuestionEntry.Questions = questionsList;

        // Save the ScriptableObject to the Assets folder
        string assetPath = "Assets/GeneratedQuestionsData.asset";
        AssetDatabase.CreateAsset(newQuestionEntry, assetPath);
        AssetDatabase.SaveAssets();
    }

    private static bool IsCSVFile(string FullPath)
    {
        return FullPath.ToLower().EndsWith(".csv");
    }
}

