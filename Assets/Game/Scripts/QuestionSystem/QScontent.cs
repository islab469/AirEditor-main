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
        // 設定本地儲存路徑
        localPath = Application.persistentDataPath + "/QDBFiles/";
        selectedExam = PlayerPrefs.GetString("selected_file", "");
        uid = FirebaseManager.GetEmail(); // ✅ 修正為正確方法

        if (!string.IsNullOrEmpty(selectedExam))
        {
            Debug.Log("📄 Selected exam exists.");
            string filePath = localPath + selectedExam;
            string cloudModifiedTime = GetCloudModifiedTime(selectedExam);

            if (File.Exists(filePath))
            {
                string localModifiedTime = File.GetLastWriteTime(filePath).ToString("yyyy-MM-dd HH:mm:ss");

                // 如果雲端檔案比較新，下載新版本
                if (string.Compare(cloudModifiedTime, localModifiedTime) > 0)
                {
                    Debug.Log("☁️ Cloud file is newer. Downloading...");
                    StartCoroutine(DownloadExam(selectedExam, uid));
                }
                else
                {
                    Debug.Log("📁 Using local file.");
                    LoadExam(filePath);
                }
            }
            else
            {
                Debug.Log("📥 No local file. Downloading...");
                StartCoroutine(DownloadExam(selectedExam, uid));
            }
        }
    }

    // 讀取本地檔案
    void LoadExam(string filePath)
    {
        string content = File.ReadAllText(filePath);
        ParseAndDisplayJson(content);
    }

    // 顯示 JSON 題目內容
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

    // 從 Django 下載考卷
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
                examText.text = "❌ Failed to download exam: " + request.error;
            }
        }
    }

    // 儲存考卷至本地
    void SaveToLocal(string filename, string content)
    {
        if (!Directory.Exists(localPath))
        {
            Directory.CreateDirectory(localPath);
        }

        string filePath = localPath + filename;
        File.WriteAllText(filePath, content);
    }

    // 從 QDBManager 中取得檔案修改時間
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

    // 匯出考題為 Word
    public void ExportToWord()
    {
        if (examText == null || string.IsNullOrEmpty(examText.text))
        {
            Debug.Log("❌ No exam content to export.");
            return;
        }

        string fileName = string.IsNullOrEmpty(selectedExam) ? "QuizExport.docx" : selectedExam + ".docx";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        SaveToWord(filePath);

        Debug.Log("✅ Exam exported to: " + filePath);
    }

    // 實作儲存至 Word 檔案
    void SaveToWord(string filePath)
    {
        try
        {
            using (var doc = DocX.Create(filePath))
            {
                doc.InsertParagraph("🧠 Quiz Content").FontSize(18).Bold().SpacingAfter(15);
                doc.InsertParagraph(examText.text).FontSize(14).SpacingAfter(10);
                doc.Save();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Failed to export Word: " + e.Message);
        }
    }
}
