using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spinner : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;

    private FirebaseFirestore firestore;

    private void Start()
    {
        if (dropdown == null)
        {
            Debug.LogError("Dropdown is not assigned in the Inspector.");
            return;
        }

        // 直接取得 Firebase Firestore 實例（不需再初始化 Firebase）
        firestore = FirebaseFirestore.DefaultInstance;

        // 開始抓選項
        StartCoroutine(GetOptionsFromFirestore());

        // 綁定選單變更事件
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    /// <summary>
    /// 從 Firestore 抓選項資料
    /// </summary>
    private IEnumerator GetOptionsFromFirestore()
    {
        string collectionPath = "uploads";

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

            if (options.Count == 0)
            {
                Debug.LogWarning("No valid image_url data found in Firestore.");
            }

            UpdateDropdownOptions(options);
        }
    }

    /// <summary>
    /// 更新下拉選單顯示
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
    /// 下拉選單選項變更時切換場景
    /// </summary>
    private void OnDropdownValueChanged(int index)
    {
        if (index >= 0 && index < dropdown.options.Count)
        {
            string selectedOption = dropdown.options[index].text;
            Debug.Log($"Selected dropdown option: {selectedOption}");

            // TODO: 可用 PlayerPrefs 傳遞選擇結果
            SceneManager.LoadScene(7);
        }
        else
        {
            Debug.LogWarning("Dropdown index out of range.");
        }
    }
}
