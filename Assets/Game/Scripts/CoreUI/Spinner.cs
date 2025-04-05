using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 加入場景管理的命名空間

public class Spinner : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;

    // Firestore 參考
    private FirebaseFirestore firestore;

    private void Start()
    {
        // 初始化 Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                firestore = FirebaseFirestore.GetInstance(app);
                Debug.Log("Firebase 初始化成功");
                StartCoroutine(GetOptionsFromFirestore());
            }
            else
            {
                Debug.LogError($"Firebase 初始化失敗: {task.Result}");
            }
        });

        // 監聽下拉選單變化
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private IEnumerator GetOptionsFromFirestore()
    {
        // Firestore 路徑
        string collectionPath = "uploads"; // Firestore 中存儲選項的集合名

        // 開始從 Firestore 中抓取資料
        CollectionReference optionsRef = firestore.Collection(collectionPath);
        var getOptionsTask = optionsRef.GetSnapshotAsync();

        // 等待 Firestore 完成資料獲取
        yield return new WaitUntil(() => getOptionsTask.IsCompleted);

        if (getOptionsTask.Exception != null)
        {
            Debug.LogError("從 Firestore 獲取資料時出現錯誤: " + getOptionsTask.Exception);
        }
        else
        {
            // 從 Firestore 獲取到資料
            QuerySnapshot snapshot = getOptionsTask.Result;

            // 將 Firestore 中的資料轉換為 List<string>
            List<string> options = new List<string>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    // 假設每個文檔中有一個 "image_url" 字段
                    string option = document.GetValue<string>("image_url");
                    options.Add(option);
                }
            }

            // 更新下拉選單選項
            UpdateDropdownOptions(options);
        }
    }

    private void UpdateDropdownOptions(List<string> options)
    {
        // 清空當前選項
        dropdown.options.Clear();

        // 添加動態選項
        foreach (string option in options)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(option));
        }

        // 設置默認選項
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    private void OnDropdownValueChanged(int index)
    {
        // 根據選中的選項進行操作
        string selectedOption = dropdown.options[index].text;
        Debug.Log($"選中的選項: {selectedOption}");

        // 替換為對應的場景名稱
        SceneManager.LoadScene(7);
    }
}
