using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Firestore;
using System.Threading.Tasks;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

public class AnswerClick : MonoBehaviour
{
    private FirebaseFirestore db;
    private DocumentReference docRef;

    // 文本元
    public TextMeshProUGUI[] textObj_arr = new TextMeshProUGUI[12];

    // 選擇題
    public TextMeshProUGUI[] textAns_arr = new TextMeshProUGUI[4];

    // 當前要填充的 textAns_arr 索引
    private int currentAnsIndex = 0; // 起始值為 0

    // 用來顯示選擇答案的文本
    public TextMeshProUGUI QuestionAndAnswer;
    public Dictionary<string, string> data = new Dictionary<string, string>();
    [SerializeField]
    private bool is_debugging = true;



    private List<object> createTestList(string frontval,int maxval) {

        List<object> strs = new List<object>();
        for (int i = 0; i < maxval; i++) {
            strs.Add(frontval + " "  + maxval.ToString());
        }
        return strs;
    }
    private async void Start()
    {
        const int q_count = 6;
        Dictionary<string, object> doc = new Dictionary<string, object>{
            { "truthes",createTestList("TRUTH", q_count)},
            { "falses",createTestList("FALSE", q_count)}
        };
        if (!is_debugging)
        {
            //MainScene.check_and_instantiate();
            string user = FirebaseManager.getEmail();
            // 初始化 Firebase
            db = FirebaseFirestore.DefaultInstance;
            //docRef = db.Collection("AIContent").Document(User);
            doc = await GetUserDocument(user);  // 調用函數來取回數據
        }

        //Load the object from firebase 
        List<object> truthesList = doc["truthes"] as List<object>;
        List<object> falsesList = doc["falses"] as List<object>;

        if (truthesList != null)
        {
            // 將每個元素轉換為字串並合併
            string truthesStr = String.Join("。", truthesList.Select(item => item.ToString()));
            truthesStr = truthesStr.Replace("\n", "").Replace("\n", "");
            string[] truthes = truthesStr.Split("。");
            print(truthes.Length);
            print(textObj_arr.Length);
            print(gameObject.name);

            for (int i = 0; i < 6; i++)
            {
                //#ERR
                textObj_arr[i].text = truthes[i];
            }
        }
        else
        {
            Debug.Log("truthes 不是 List<object> 類型");
        }

        if (falsesList != null)
        {
            // 將每個元素轉換為字串並合併

            string falsesStr = String.Join("。", falsesList.Select(item => item.ToString()));
            falsesStr = falsesStr.Replace("\n", "").Replace("\r", "");
            string[] falses = falsesStr.Split("。");
            for (int i = 0; i < 6; i++)
            {
                textObj_arr[i+6].text = falses[i];
            }
            // 顯示合併後的字串
        }
        else
        {
            Debug.Log("truthes 不是 List<object> 類型");
        }



        //for (int i = 0; i < textObj_arr.Length; i++)
        //{
        //    textObj_arr[i].text = 
        //    Debug.Log(strs[i]);
        //}

    }

    private async Task<Dictionary<string, object>> GetUserDocument(string userId)
    {
        
        // 指向 Firestore 的 AIContent 集合內的 userId 文檔
        DocumentReference docRef = db.Collection("AIContent").Document(userId);
     
        Dictionary<string, object> documentData = new Dictionary<string, object>();


        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            // 取出文檔的數據，並將其轉換為字典
            documentData = snapshot.ToDictionary();

        }


        return documentData;
    }

    // 用來在按鈕點擊後讀取文本並顯示的函數
    public void OnAnswerClick(int answerIndex)
    {
        // 檢查索引是否在範圍內
        if (answerIndex >= 0 && answerIndex < textObj_arr.Length)
        {
            // 獲取選中的文本
            string selectedText = textObj_arr[answerIndex].text;

            // 檢查 textAns_arr 是否還有未填入的文本
            if (currentAnsIndex < textAns_arr.Length)
            {
                // 將選中的文本複製到 textAns_arr 的當前索引
                textAns_arr[currentAnsIndex].text = selectedText;

                // 更新索引，準備下一次點擊
                currentAnsIndex++;
            }
            else
            {
                Debug.LogWarning("All answer slots are filled.");
            }
        }
        else
        {
            Debug.LogError("Index is out of bounds: " + answerIndex);
        }
    }
}
