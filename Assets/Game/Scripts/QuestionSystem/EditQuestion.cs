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
    public TMP_InputField mainInput; // ��@��J��
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
        // �ǳƸ��|�P���
        localPath = Path.Combine(Application.persistentDataPath, "QDBFiles");
        selectedExam = PlayerPrefs.GetString("selected_file", "");
        LoadExam(Path.Combine(localPath, selectedExam));

        // �j�w���s�ƥ�
        prevButton.onClick.AddListener(PreviousQuestion);
        nextButton.onClick.AddListener(NextQuestion);
        saveButton.onClick.AddListener(SaveExam);

        // ��ť��J����
        mainInput.onEndEdit.AddListener(UpdateQuestion);

        DisplayQuestion();
    }

    // ���J JSON �ɮצ��� QuestionList
    void LoadExam(string filePath)
    {
        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            questionList = JsonUtility.FromJson<QuestionList>(content);
        }
        else
        {
            Debug.LogWarning("Exam file not found at: " + filePath);
        }
    }

    // ��ܥثe�D��
    void DisplayQuestion()
    {
        if (questionList != null && questionList.Question.Count > 0)
        {
            var q = questionList.Question[currentIndex];

            // �X�֬���J�ئr��
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(q.Title);

            for (int i = 0; i < q.Options.Count; i++)
            {
                string option = q.Options[i].Trim();
                char prefix = (char)('A' + i);

                if (!option.StartsWith("(A)") && !option.StartsWith("(B)") &&
                    !option.StartsWith("(C)") && !option.StartsWith("(D)"))
                {
                    option = $"({prefix}) {option}";
                }

                builder.AppendLine(option);
            }

            builder.Append(q.Ans);
            mainInput.text = builder.ToString();
        }
    }

    // ��ק��J�ث��s��e�D�ؤ��e
    void UpdateQuestion(string newText)
    {
        if (questionList != null && questionList.Question.Count > 0)
        {
            var lines = newText.Split('\n');
            if (lines.Length < 2) return;

            var q = questionList.Question[currentIndex];
            q.Title = lines[0].Trim();
            q.Options.Clear();

            for (int i = 1; i < lines.Length - 1; i++)
            {
                string option = lines[i].Trim();
                if (!string.IsNullOrEmpty(option))
                    q.Options.Add(option);
            }

            q.Ans = lines[^1].Trim();
        }
    }

    // �W�@�D
    void PreviousQuestion()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            DisplayQuestion();
        }
    }

    // �U�@�D
    void NextQuestion()
    {
        if (currentIndex < questionList.Question.Count - 1)
        {
            currentIndex++;
            DisplayQuestion();
        }
    }

    // �x�s�äW�ǦҨ�
    void SaveExam()
    {
        if (questionList == null)
        {
            Debug.LogWarning("No question data to save.");
            return;
        }

        string uid = FirebaseManager.GetEmail();
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("No user logged in.");
            return;
        }

        string jsonData = JsonUtility.ToJson(questionList);
        string filename = selectedExam;

        Debug.Log("Uploading exam: " + filename);
        StartCoroutine(UploadExam(uid, filename, jsonData));
    }

    // �o�e HTTP POST �W�Ǹ�Ʀ� Django
    IEnumerator UploadExam(string uid, string filename, string jsonData)
    {
        string payload = $"{{\"uid\":\"{uid}\", \"filename\":\"{filename}\", \"data\":{jsonData}}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);

        using (UnityWebRequest request = new UnityWebRequest(djangoUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Exam uploaded successfully.");
            }
            else
            {
                Debug.LogError("Exam upload failed: " + request.error);
            }
        }
    }
}
