using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Threading.Tasks;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;

public class AnswerClick : MonoBehaviour
{
    private FirebaseFirestore db;
    private DocumentReference docRef;

    // 12 個原始句子（正確與錯誤）
    public TextMeshProUGUI[] textObj_arr = new TextMeshProUGUI[12];

    // 4 個答案區塊
    public TextMeshProUGUI[] textAns_arr = new TextMeshProUGUI[4];

    // 當前要填入的答案區塊索引
    private int currentAnsIndex = 0;

    // 顯示問題與答案的元件
    public TextMeshProUGUI QuestionAndAnswer;

    // 假資料用的除錯旗標
    [SerializeField]
    private bool is_debugging = true;

    // 產生測試資料
    private List<object> CreateTestList(string prefix, int count)
    {
        List<object> results = new List<object>();
        for (int i = 0; i < count; i++)
        {
            results.Add($"{prefix} {i + 1}");
        }
        return results;
    }

    private async void Start()
    {
        const int q_count = 6;

        Dictionary<string, object> doc = new Dictionary<string, object>
        {
            { "truthes", CreateTestList("TRUTH", q_count) },
            { "falses", CreateTestList("FALSE", q_count) }
        };

        // 連接 Firebase Firestore
        if (!is_debugging)
        {
            string user = FirebaseManager.GetEmail();
            db = FirebaseFirestore.DefaultInstance;
            doc = await GetUserDocument(user);
        }

        List<object> truthesList = doc.ContainsKey("truthes") ? doc["truthes"] as List<object> : null;
        List<object> falsesList = doc.ContainsKey("falses") ? doc["falses"] as List<object> : null;

        // 填入正確句子
        if (truthesList != null)
        {
            string[] truthes = string.Join("。", truthesList.Select(t => t.ToString()))
                .Replace("\n", "")
                .Replace("\r", "")
                .Split("。", StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < Mathf.Min(6, truthes.Length); i++)
            {
                textObj_arr[i].text = truthes[i];
            }
        }
        else
        {
            Debug.LogWarning("Missing or invalid 'truthes' list from Firestore.");
        }

        // 填入錯誤句子
        if (falsesList != null)
        {
            string[] falses = string.Join("。", falsesList.Select(f => f.ToString()))
                .Replace("\n", "")
                .Replace("\r", "")
                .Split("。", StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < Mathf.Min(6, falses.Length); i++)
            {
                textObj_arr[i + 6].text = falses[i];
            }
        }
        else
        {
            Debug.LogWarning("Missing or invalid 'falses' list from Firestore.");
        }
    }

    // 從 Firestore 取得使用者資料
    private async Task<Dictionary<string, object>> GetUserDocument(string userId)
    {
        db ??= FirebaseFirestore.DefaultInstance;

        DocumentReference docRef = db.Collection("AIContent").Document(userId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ToDictionary();
        }
        else
        {
            Debug.LogError("Document not found for user: " + userId);
            return new Dictionary<string, object>();
        }
    }

    // 當點擊某一答案選項時執行
    public void OnAnswerClick(int answerIndex)
    {
        if (answerIndex >= 0 && answerIndex < textObj_arr.Length)
        {
            string selectedText = textObj_arr[answerIndex].text;

            if (currentAnsIndex < textAns_arr.Length)
            {
                textAns_arr[currentAnsIndex].text = selectedText;
                currentAnsIndex++;
            }
            else
            {
                Debug.LogWarning("All answer slots are already filled.");
            }
        }
        else
        {
            Debug.LogError("Invalid answer index: " + answerIndex);
        }
    }
}
