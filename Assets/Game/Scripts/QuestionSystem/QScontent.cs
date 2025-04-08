using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Xceed.Words.NET; // 使用 DocX 套件
using Application = UnityEngine.Application;

[System.Serializable]
public class Question
{
    public string Title;
    public List<string> Options;
    public string Ans;
    public string Answer;
    public int type;
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
    private string djangoUrl = "http://120.101.10.105:8000/unitydata/get_exam/";

    void Start()
    {
        localPath = Application.persistentDataPath + "/QDBFiles/";
        selectedExam = PlayerPrefs.GetString("selected_file", "");
        uid = FirebaseManager.GetEmail();

        Debug.Log("Init path: " + localPath);
        Debug.Log("Selected exam: " + selectedExam);
        Debug.Log("User ID: " + uid);

        if (!string.IsNullOrEmpty(selectedExam))
        {
            print("selectedExam 有資料");
            string filePath = localPath + selectedExam;

            string cloudModifiedTime = GetCloudModifiedTime(selectedExam);

            if (File.Exists(filePath))
            {
                string localModifiedTime = File.GetLastWriteTime(filePath).ToString("yyyy-MM-dd HH:mm:ss");

                if (string.Compare(cloudModifiedTime, localModifiedTime) > 0)
                {
                    print("檔案有更新，下載新檔案");
                    StartCoroutine(DownloadExam(selectedExam, uid));
                }
                else
                {
                    print("本地檔案已是最新，直接載入");
                    LoadExam(filePath);
                }
            }
            else
            {
                print("本地無檔案，下載題目");
                StartCoroutine(DownloadExam(selectedExam, uid));
            }
        }
    }

    void LoadExam(string filePath)
    {
        Debug.Log("Loading exam from file: " + filePath);
        if (!File.Exists(filePath))
        {
            Debug.LogError("File does not exist: " + filePath);
            return;
        }

        string content = File.ReadAllText(filePath);
        Debug.Log("Loaded content: " + content.Substring(0, Mathf.Min(content.Length, 100)) + "...");
        ParseAndDisplayJson(content);
    }

    void ParseAndDisplayJson(string jsonData)
    {
        Debug.Log("Parsing JSON...");
        QuestionList data = JsonUtility.FromJson<QuestionList>(jsonData);

        if (data != null && data.Question != null)
        {
            string displayText = "";
            foreach (Question q in data.Question)
            {
                print("type"+q.type);
                displayText += q.Title + "\n";
                
                switch (q.type)
                {
                    case 2: // 選擇題
                        if (q.Options != null && q.Options.Count > 0)
                        {
                            foreach (string opt in q.Options)
                            {
                                displayText += opt + "\n";
                            }
                        }
                        else
                        {
                            displayText += "(⚠️ 選擇題缺少選項)\n";
                        }
                        break;

                    case 0: // 是非題
                        
                        break;

                    case 4: // 問答題
                        
                        break;

                    default:
                        displayText += "(⚠️ 未知題型)\n";
                        break;
                }

                displayText += "----------------------\n";
            }
            print(displayText);
            examText.text = displayText;
            Debug.Log("Parsed and displayed exam.");
        }
        else
        {
            Debug.LogWarning("Parsed JSON is empty or malformed.");
        }
    }

    IEnumerator DownloadExam(string filename, string uid)
    {
        string fullUrl = djangoUrl + "?uid=" + uid + "&filename=" + filename;
        Debug.Log("Downloading from URL: " + fullUrl);

        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Download success.");
                string fileContent = request.downloadHandler.text;
                SaveToLocal(filename, fileContent);
                LoadExam(localPath + filename);
            }
            else
            {
                Debug.LogError("Download failed: " + request.error);
                examText.text = "Download failed: " + request.error;
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
        Debug.Log("Saved file to: " + filePath);
    }

    string GetCloudModifiedTime(string filename)
    {
        foreach (var file in QDBManager.FileList)
        {
            if (file.filename == filename)
            {
                Debug.Log("Cloud modified time matched: " + file.modified_time);
                return file.modified_time;
            }
        }
        Debug.LogWarning("No cloud timestamp found. Returning fallback.");
        return "2000-01-01 00:00:00";
    }

    public void ExportToWord()
    {
        if (examText == null || string.IsNullOrEmpty(examText.text))
        {
            Debug.Log("No content to export.");
            return;
        }

        string fileName = selectedExam + ".docx";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        SaveToWord(filePath);
        Debug.Log("Exported to: " + filePath);
    }

    void SaveToWord(string filePath)
    {
        using (var doc = DocX.Create(filePath))
        {
            doc.InsertParagraph(examText.text);
            doc.Save();
        }
    }
}
