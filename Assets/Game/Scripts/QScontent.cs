using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Windows.Forms; 
using Microsoft.Office.Interop.Word; 
using System.Reflection;
using Application = UnityEngine.Application;
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
            print("selectedExam �����");
            string filePath = localPath + selectedExam;

            string cloudModifiedTime = GetCloudModifiedTime(selectedExam);

            if (File.Exists(filePath))
            {
                string localModifiedTime = File.GetLastWriteTime(filePath).ToString("yyyy-MM-dd HH:mm:ss");

                if (string.Compare(cloudModifiedTime, localModifiedTime) > 0)
                {
                    print("�ɮצ���s�A�U���s�ɮ�");
                    StartCoroutine(DownloadExam(selectedExam, uid));
                }
                else
                {
                    print("���a�ɮ�");
                    print(localModifiedTime);
                    LoadExam(filePath);
                }
            }
            else
            {
                print("���a�L�ɮסA�U���D��");
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
                    displayText += option + "\n";
                }
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
                examText.text = "�Ҩ��U������: " + request.error;
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
                print("���ݭק�ɶ�");
                print(file.modified_time);
                return file.modified_time;
            }
        }
        return "2000-01-01 00:00:00";
    }
    public void ExportToWord()
    {
        if (examText == null || string.IsNullOrEmpty(examText.text))
        {
            print("�S�����D�i�ץX");
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "Word ��� (*.docx)|*.docx",
            Title = "��ܶץX�ɮצ�m",
            FileName = selectedExam
        };

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            string filePath = saveFileDialog.FileName;
            SaveToWord(filePath);
            print("���D�w�ץX�ܡG" + filePath);
        }
    }

    void SaveToWord(string filePath)
    {
        Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
        Microsoft.Office.Interop.Word.Document wordDoc = wordApp.Documents.Add();

        // �N���D���e�g�J Word
        Microsoft.Office.Interop.Word.Paragraph para = wordDoc.Content.Paragraphs.Add();
        para.Range.Text = examText.text;
        para.Range.InsertParagraphAfter();

        // �x�s Word �ɮ�
        wordDoc.SaveAs2(filePath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocumentDefault);
        wordDoc.Close();
        wordApp.Quit();
    }
}
