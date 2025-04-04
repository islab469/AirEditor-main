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
    public TMP_InputField mainInput; // �u���@�ӿ�J��
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

            // �N�D�ءB�ﶵ�B���׮榡�Ƭ��@�ӿ�J�ت����e
            string formattedText = currentQuestion.Title + "\n";
            for (int i = 0; i < currentQuestion.Options.Count; i++)
            {
                string optionText = currentQuestion.Options[i].Trim();

                // �קK�ﶵ���ӴN�� "(A)" �o���e��
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
            if (lines.Length < 2) return; // �T�O�ܤ֦��D�ةM�@�ӿﶵ

            var currentQuestion = questionList.Question[currentIndex];

            currentQuestion.Title = lines[0]; // �Ĥ@��O�D��
            currentQuestion.Options.Clear();

            for (int i = 1; i < lines.Length - 1; i++) // �����X��O�ﶵ
            {
                string optionText = lines[i].Trim();
                if (!string.IsNullOrEmpty(optionText))
                {
                    currentQuestion.Options.Add(optionText);
                }
            }

            currentQuestion.Ans = lines[^1]; // �̫�@��O����
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

        print("�W�ǦҨ��G" + filename);
        StartCoroutine(UploadExam(uid, filename, jsonData));
    }

    IEnumerator UploadExam(string uid, string filename, string jsonData)
    {

        // �إ� JSON Payload
        string jsonPayload = "{\"uid\":\"" + uid + "\", \"filename\":\"" + filename + "\", \"data\":" + jsonData + "}";

        byte[] postData = Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(djangoUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");  // �]�w JSON Header

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("�Ҩ���s���\�I");
            }
            else
            {
                Debug.LogError("�Ҩ���s����: " + request.error);
            }
        }
    }
}
