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
        if (Selection.activeObject == null) return;

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

    private static void PerformGeneration(List <Dictionary<string, object>> csvData)
    {
        for (int i = 0; i < csvData.Count; i++)
        {
            Dictionary<string, object> _potentialQuestionEntry = csvData[i];
            
            CreateScriptableObjectQuestionData(_potentialQuestionEntry["QUESTION"].ToString(), _potentialQuestionEntry["DIFFICULTY"].ToString(), _potentialQuestionEntry["CORRECT ANSWER"].ToString());
            
            Debug.Log($"Question : {_potentialQuestionEntry["QUESTION"]}");
            Debug.Log($"Difficulty : {_potentialQuestionEntry["DIFFICULTY"]}");
            Debug.Log($"Correct Answer : {_potentialQuestionEntry["CORRECT ANSWER"]}");

            if(i > 8) break;
        }
    }

    private static void CreateScriptableObjectQuestionData(string questionText, string difficulty, string correctAnswer)
    {
        QuestionsData newQuestionEntry = ScriptableObject.CreateInstance<QuestionsData>();

        // Initialize the Questions list if it is null
        if (newQuestionEntry.Questions == null)
        {
            newQuestionEntry.Questions = new List<QuestionsData.Question>();
        }

        // Create a new Question struct instance and set its fields
        QuestionsData.Question newQuestion = new QuestionsData.Question
        {
            questionText = questionText,
            difficulty = difficulty,
            // choices = new string[] { }, // You would need to set the choices as well
            correctAnswer = correctAnswer,
            // correctChoiceIndex = correctChoiceIndex
        };

        // Add the new Question to the Questions list
        newQuestionEntry.Questions.Add(newQuestion);

        // Uncomment these lines to create and save the asset if this method is used within Unity Editor scripts
        AssetDatabase.CreateAsset(newQuestionEntry, $"Assets/Resources/QuestionData/{questionText}.asset");
        AssetDatabase.SaveAssets();
    }

    private static bool IsCSVFile(string FullPath)
    {
        return FullPath.ToLower().EndsWith(".csv");
    }
}