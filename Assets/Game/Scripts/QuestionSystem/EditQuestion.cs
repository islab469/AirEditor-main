using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class EditQuestion : MonoBehaviour
{
    public TMP_InputField mainInput; // 只有一個輸入框
    public Button prevButton;
    public Button nextButton;
    public Button saveButton;

    private QuestionList questionList;
    private int currentIndex = 0;
    private string localPath;
    private string selectedExam;
    private string djangoUrl = "http://127.0.0.1:8000/unitydata/upload_exam/";

    void Start()
    {
        localPath = Application.persistentDataPath + "/QDBFiles/";
        selectedExam = PlayerPrefs.GetString("selected_file", "");
        LoadExam(localPath + selectedExam);

        prevButton.onClick.AddListener(PreviousQuestion);
        nextButton.onClick.AddListener(NextQuestion);
        saveButton.onClick.AddListener(SaveExam);

        mainInput.onEndEdit.AddListener(UpdateQuestion);

        DisplayQuestion();
    }

    void LoadExam(string filePath)
    {
        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            questionList = JsonUtility.FromJson<QuestionList>(content);
        }
    }

    void DisplayQuestion()
    {
        if (questionList != null && questionList.Question.Count > 0)
        {
            var currentQuestion = questionList.Question[currentIndex];

            // 將題目、選項、答案格式化為一個輸入框的內容
            string formattedText = currentQuestion.Title + "\n";
            for (int i = 0; i < currentQuestion.Options.Count; i++)
            {
                string optionText = currentQuestion.Options[i].Trim();

                // 避免選項本來就有 "(A)" 這類前綴
                if (!optionText.StartsWith("(A)") && !optionText.StartsWith("(B)") &&
                    !optionText.StartsWith("(C)") && !optionText.StartsWith("(D)"))
                {
                    char optionChar = (char)('A' + i);
                    optionText = $"({optionChar}) {optionText}";
                }

                formattedText += optionText + "\n";
            }
            formattedText += currentQuestion.Ans;

            mainInput.text = formattedText;
        }
    }

    void UpdateQuestion(string newText)
    {
        if (questionList != null && questionList.Question.Count > 0)
        {
            var lines = newText.Split('\n');
            if (lines.Length < 2) return; // 確保至少有題目和一個選項

            var currentQuestion = questionList.Question[currentIndex];

            currentQuestion.Title = lines[0]; // 第一行是題目
            currentQuestion.Options.Clear();

            for (int i = 1; i < lines.Length - 1; i++) // 中間幾行是選項
            {
                string optionText = lines[i].Trim();
                if (!string.IsNullOrEmpty(optionText))
                {
                    currentQuestion.Options.Add(optionText);
                }
            }

            currentQuestion.Ans = lines[^1]; // 最後一行是答案
        }
    }

    void PreviousQuestion()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            DisplayQuestion();
        }
    }

    void NextQuestion()
    {
        if (currentIndex < questionList.Question.Count - 1)
        {
            currentIndex++;
            DisplayQuestion();
        }
    }

    void SaveExam()
    {
        if (questionList == null) return;

        string uid = FirebaseManager.getEmail();
        string jsonData = JsonUtility.ToJson(questionList);
        string filename = selectedExam;

        print("上傳考卷：" + filename);
        StartCoroutine(UploadExam(uid, filename, jsonData));
    }

    IEnumerator UploadExam(string uid, string filename, string jsonData)
    {

        // 建立 JSON Payload
        string jsonPayload = "{\"uid\":\"" + uid + "\", \"filename\":\"" + filename + "\", \"data\":" + jsonData + "}";

        byte[] postData = Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(djangoUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");  // 設定 JSON Header

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("考卷更新成功！");
            }
            else
            {
                Debug.LogError("考卷更新失敗: " + request.error);
            }
        }
    }
}
