using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Xceed.Words.NET;

[System.Serializable]
public class Question
{
    public string Title;
    public List<string> Options;
    public string Ans;
}

[System.Serializable]
public class QuestionList
{
    public List<Question> Question;
}

public class QScontent : MonoBehaviour
{
    public TextMeshProUGUI examText;
    private string localPath;
    private string uid;
    private string selectedExam;
    private string djangoUrl = "http://127.0.0.1:8000/unitydata/get_exam/";

    void Start()
    {
        localPath = Application.persistentDataPath + "/QDBFiles/";
        selectedExam = PlayerPrefs.GetString("selected_file", "");
        uid = FirebaseManager.getEmail();

        if (!string.IsNullOrEmpty(selectedExam))
        {
            Debug.Log("selectedExam 有資料");
            string filePath = localPath + selectedExam;
            string cloudModifiedTime = GetCloudModifiedTime(selectedExam);

            if (File.Exists(filePath))
            {
                string localModifiedTime = File.GetLastWriteTime(filePath).ToString("yyyy-MM-dd HH:mm:ss");

                if (string.Compare(cloudModifiedTime, localModifiedTime) > 0)
                {
                    Debug.Log("檔案有更新，下載新檔案");
                    StartCoroutine(DownloadExam(selectedExam, uid));
                }
                else
                {
                    Debug.Log("使用本地檔案");
                    LoadExam(filePath);
                }
            }
            else
            {
                Debug.Log("本地無檔案，下載題目");
                StartCoroutine(DownloadExam(selectedExam, uid));
            }
        }
    }

    void LoadExam(string filePath)
    {
        string content = File.ReadAllText(filePath);
        ParseAndDisplayJson(content);
    }

    void ParseAndDisplayJson(string jsonData)
    {
        QuestionList data = JsonUtility.FromJson<QuestionList>(jsonData);
        if (data != null && data.Question != null)
        {
            string displayText = "";
            foreach (Question q in data.Question)
            {
                displayText += q.Title + "\n";
                foreach (string option in q.Options)
                {
                    displayText += "- " + option + "\n";
                }
                displayText += "Ans: " + q.Ans + "\n";
                displayText += "----------------------\n";
            }
            examText.text = displayText;
        }
    }

    IEnumerator DownloadExam(string filename, string uid)
    {
        string fullUrl = djangoUrl + "?uid=" + uid + "&filename=" + filename;
        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string fileContent = request.downloadHandler.text;
                SaveToLocal(filename, fileContent);
                LoadExam(localPath + filename);
            }
            else
            {
                examText.text = "考卷下載失敗: " + request.error;
            }
        }
    }

    void SaveToLocal(string filename, string content)
    {
        if (!Directory.Exists(localPath))
        {
            Directory.CreateDirectory(localPath);
        }

        string filePath = localPath + filename;
        File.WriteAllText(filePath, content);
    }

    string GetCloudModifiedTime(string filename)
    {
        foreach (var file in QDBManager.FileList)
        {
            if (file.filename == filename)
            {
                return file.modified_time;
            }
        }
        return "2000-01-01 00:00:00";
    }

    public void ExportToWord()
    {
        if (examText == null || string.IsNullOrEmpty(examText.text))
        {
            Debug.Log("❌ 沒有考題可匯出");
            return;
        }

        string fileName = string.IsNullOrEmpty(selectedExam) ? "QuizExport.docx" : selectedExam + ".docx";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        SaveToWord(filePath);

        Debug.Log("✅ 考題已匯出至：" + filePath);
    }

    void SaveToWord(string filePath)
    {
        try
        {
            using (var doc = DocX.Create(filePath))
            {
                doc.InsertParagraph("🧠 題目內容").FontSize(18).Bold().SpacingAfter(15);
                doc.InsertParagraph(examText.text).FontSize(14).SpacingAfter(10);
                doc.Save();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ 匯出 Word 失敗：" + e.Message);
        }
    }
}
