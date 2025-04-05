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

    // 12 �ӭ�l�y�l�]���T�P���~�^
    public TextMeshProUGUI[] textObj_arr = new TextMeshProUGUI[12];

    // 4 �ӵ��װ϶�
    public TextMeshProUGUI[] textAns_arr = new TextMeshProUGUI[4];

    // ��e�n��J�����װ϶�����
    private int currentAnsIndex = 0;

    // ��ܰ��D�P���ת�����
    public TextMeshProUGUI QuestionAndAnswer;

    // ����ƥΪ������X��
    [SerializeField]
    private bool is_debugging = true;

    // ���ʹ��ո��
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

        // �s�� Firebase Firestore
        if (!is_debugging)
        {
            string user = FirebaseManager.GetEmail();
            db = FirebaseFirestore.DefaultInstance;
            doc = await GetUserDocument(user);
        }

        List<object> truthesList = doc.ContainsKey("truthes") ? doc["truthes"] as List<object> : null;
        List<object> falsesList = doc.ContainsKey("falses") ? doc["falses"] as List<object> : null;

        // ��J���T�y�l
        if (truthesList != null)
        {
            string[] truthes = string.Join("�C", truthesList.Select(t => t.ToString()))
                .Replace("\n", "")
                .Replace("\r", "")
                .Split("�C", StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < Mathf.Min(6, truthes.Length); i++)
            {
                textObj_arr[i].text = truthes[i];
            }
        }
        else
        {
            Debug.LogWarning("Missing or invalid 'truthes' list from Firestore.");
        }

        // ��J���~�y�l
        if (falsesList != null)
        {
            string[] falses = string.Join("�C", falsesList.Select(f => f.ToString()))
                .Replace("\n", "")
                .Replace("\r", "")
                .Split("�C", StringSplitOptions.RemoveEmptyEntries);

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

    // �q Firestore ���o�ϥΪ̸��
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

    // ���I���Y�@���׿ﶵ�ɰ���
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
