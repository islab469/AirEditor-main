using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spinner : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;

    // Firestore 實例
    private FirebaseFirestore firestore;

    private void Start()
    {
        // 初始化 Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                firestore = FirebaseFirestore.GetInstance(app);
                Debug.Log("Firebase initialized successfully.");
                StartCoroutine(GetOptionsFromFirestore());
            }
            else
            {
                Debug.LogError($"Firebase initialization failed: {task.Result}");
            }
        });

        // 下拉選單事件綁定
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    /// <summary>
    /// 從 Firestore 取得所有上傳資料
    /// </summary>
    private IEnumerator GetOptionsFromFirestore()
    {
        string collectionPath = "uploads"; // Firestore 資料集合

        CollectionReference optionsRef = firestore.Collection(collectionPath);
        var getOptionsTask = optionsRef.GetSnapshotAsync();

        yield return new WaitUntil(() => getOptionsTask.IsCompleted);

        if (getOptionsTask.Exception != null)
        {
            Debug.LogError("Error fetching data from Firestore: " + getOptionsTask.Exception);
        }
        else
        {
            QuerySnapshot snapshot = getOptionsTask.Result;

            List<string> options = new List<string>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists && document.ContainsField("image_url"))
                {
                    string option = document.GetValue<string>("image_url");
                    options.Add(option);
                }
            }

            UpdateDropdownOptions(options);
        }
    }

    /// <summary>
    /// 將選項加入下拉選單
    /// </summary>
    private void UpdateDropdownOptions(List<string> options)
    {
        dropdown.options.Clear();

        foreach (string option in options)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(option));
        }

        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    /// <summary>
    /// 當選擇變更時觸發的事件
    /// </summary>
    private void OnDropdownValueChanged(int index)
    {
        string selectedOption = dropdown.options[index].text;
        Debug.Log($"Selected dropdown option: {selectedOption}");

        // 切換到第 7 號場景（請依實際場景調整）
        SceneManager.LoadScene(7);
    }
}
