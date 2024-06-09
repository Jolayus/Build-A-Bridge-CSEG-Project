using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CSVImportMenu : MonoBehaviour
{
    [MenuItem("CSV Import/Generate Question Data")]
    public static void CSVGenerate()
    {
        if (Selection.activeObject == null)
        {
            Debug.Log("Please select a CSV file");
            return;
        }

        string path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(Selection.activeObject));
        string fileName = System.IO.Path.GetFileName(path);

        if (!IsCSVFile(path))
        {
            Debug.Log("Please select a CSV file");
            return;
        }

        List<Dictionary<string, object>> rawCSVData = CSVReading.CSVReader.Read(path);

        if (rawCSVData.Count > 0)
        {
            bool confirmed = EditorUtility.DisplayDialog("Generate Question Data", $"Are you sure you want to generate {rawCSVData.Count} question data from the selected CSV file?", "Yes", "No");

            if (confirmed)
            {
                if (fileName.ToLower().Contains("easy") || fileName.ToLower().Contains("medium"))
                {
                    PerformGenerationEasyMedium(rawCSVData);
                }
                else if (fileName.ToLower().Contains("hard"))
                {
                    PerformGenerationHard(rawCSVData);
                }
            }
        }
        else
        {
            Debug.Log("No Entries in the selected CSV file");
            return;
        }
    }

    private static void PerformGenerationEasyMedium(List<Dictionary<string, object>> csvData)
    {
        QuestionsData questionsDataNewEntry = ScriptableObject.CreateInstance<QuestionsData>();

        for (int i = 0; i < csvData.Count; i++)
        {
            Dictionary<string, object> _potentialQuestionEntry = csvData[i];

            string questionText = _potentialQuestionEntry["QUESTION"].ToString();
            string difficulty = _potentialQuestionEntry["DIFFICULTY"].ToString();
            string correctAnswer = _potentialQuestionEntry["CORRECT ANSWER"].ToString();

            QuestionsData.QuestionSimple simpleQuestion = new QuestionsData.QuestionSimple
            {
                questionText = questionText,
                difficulty = difficulty,
                correctAnswer = correctAnswer,
            };

            questionsDataNewEntry.simpleQuestions.Add(simpleQuestion);

            Debug.Log($"Question added: {questionText}");
        }

        Debug.Log($"Total questions generated: {questionsDataNewEntry.simpleQuestions.Count}");
        CreateScriptableObjectQuestionData(questionsDataNewEntry);
    }

    private static void PerformGenerationHard(List<Dictionary<string, object>> csvData)
    {
        QuestionsData questionsDataNewEntry = ScriptableObject.CreateInstance<QuestionsData>();

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

            QuestionsData.QuestionMultipleChoice mcQuestion = new QuestionsData.QuestionMultipleChoice
            {
                questionText = questionText,
                difficulty = difficulty,
                correctAnswer = correctAnswer,
                choices = choices,
                correctChoiceIndex = correctChoiceIndex
            };

            questionsDataNewEntry.multipleChoiceQuestions.Add(mcQuestion);

            Debug.Log($"Question added: {questionText}");
        }

        Debug.Log($"Total questions generated: {questionsDataNewEntry.multipleChoiceQuestions.Count}");
        CreateScriptableObjectQuestionData(questionsDataNewEntry);
    }

    private static void CreateScriptableObjectQuestionData(QuestionsData questionsData)
    {
        // Save the ScriptableObject to the Assets folder
        string assetPath = $"Assets/Scripts/CSV DATA/QuestionData.asset";
        AssetDatabase.CreateAsset(questionsData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created asset with {questionsData.multipleChoiceQuestions.Count} questions at {assetPath}");
    }

    private static bool IsCSVFile(string FullPath)
    {
        return FullPath.ToLower().EndsWith(".csv");
    }
}
