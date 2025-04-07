using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class ContentCtrler : MonoBehaviour
{
    public Transform content;
    public GameObject projectPrefab;
    private FirebaseFirestore db;

    void Start()
    {
        Debug.Log("📦 ContentCtrler Start()");
        db = FirebaseFirestore.DefaultInstance;
        LoadImage();
    }

    void LoadImage()
    {
        string email = FirebaseManager.GetEmail();
        Debug.Log("🔍 取得 Firebase Email: " + email);

        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("❌ FirebaseManager.GetEmail() 回傳空值，無法連接 Firestore Document！");
            return;
        }

        db.Collection("users").Document(email).Collection("uploads").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ Firestore 錯誤：" + task.Exception);
                return;
            }

            if (!task.Result.Any())
            {
                Debug.LogWarning("⚠️ Firestore 中沒有上傳的資料");
                return;
            }

            Debug.Log($"📸 找到 {task.Result.Count} 筆上傳資料");

            foreach (DocumentSnapshot document in task.Result.Documents)
            {
                if (document.Exists)
                {
                    Dictionary<string, object> data = document.ToDictionary();
                    Debug.Log($"📝 讀取到 document：{document.Id}");

                    if (data.ContainsKey("image_url"))
                    {
                        string imageUrl = data["image_url"].ToString();
                        Debug.Log("✅ 找到圖片 URL：" + imageUrl);
                        CreatePrefab(imageUrl);
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ 該 document 中沒有 'image_url'");
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ 有 document 不存在");
                }
            }
        });
    }

    void CreatePrefab(string imageUrl)
    {
        Debug.Log("🧩 建立 Prefab 中...");
        GameObject newProjectPrefab = Instantiate(projectPrefab, content);
        Debug.Log("✅ 成功生成 Prefab 並加入 ScrollView");

        Button button = newProjectPrefab.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnPrefabClicked(imageUrl));
        }
        else
        {
            Debug.LogWarning("⚠️ 找不到 Button 組件於 Prefab 中");
        }

        Image imageComponent = newProjectPrefab.GetComponent<Image>();
        if (imageComponent != null)
        {
            Debug.Log("🖼️ 嘗試載入圖片...");
            StartCoroutine(LoadImage(imageUrl, imageComponent));
        }
        else
        {
            Debug.LogError("❌ Prefab 沒有 Image 組件，請檢查 Prefab 結構");
        }

        MoveAddButtonToEnd();
    }

    void OnPrefabClicked(string imageUrl)
    {
        Debug.Log("🖱️ 點擊了 Prefab，將儲存 URL 並跳轉場景：" + imageUrl);
        PlayerPrefs.SetString("SelectedImageURL", imageUrl);
        SceneManager.LoadScene(1);
    }

    IEnumerator LoadImage(string url, Image imageComponent)
    {
        Debug.Log("🌐 下載圖片中：" + url);
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageComponent.sprite = sprite;
                Debug.Log("✅ 圖片載入成功！");
            }
            else
            {
                Debug.LogError("❌ 圖片載入失敗：" + request.error);
            }
        }
    }

    void MoveAddButtonToEnd()
    {
        Transform addButton = content.Find("Image"); // 確保你的加號按鈕名稱是 "Image"
        if (addButton != null)
        {
            addButton.SetAsLastSibling();
            Debug.Log("🔁 加號按鈕移到最尾端");
        }
        else
        {
            Debug.LogWarning("⚠️ 找不到名稱為 'Image' 的加號按鈕");
        }
    }
}
