using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;


public class ContentC : MonoBehaviour
{
    public Transform content;
    public GameObject projectPrefab;
    private FirebaseFirestore db;
    private DocumentReference docRef;
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        LoadImage();

    }

    void LoadImage()
    {
        string email = FirebaseManager.getEmail();
        db.Collection("users").Document(email).Collection("uploads").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;
                print("找到n個上傳的圖片文件");
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        Dictionary<string, object> data = document.ToDictionary();
                        if (data.ContainsKey("image_url"))
                        {
                            string imageUrl = data["image_url"].ToString();
                            Debug.Log("找到圖片 URL：" + imageUrl);
                            CreatePrefab(imageUrl);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Firestore 讀取失敗：" + task.Exception);
            }
        });

    }

    void CreatePrefab(string imageUrl)
    {
        GameObject newProjectPrefab = Instantiate(projectPrefab, content);
        Debug.Log("成功生成 Prefab 並加入 ScrollView");
        Button button = newProjectPrefab.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnPrefabClicked(imageUrl));
        }

        // 在 Prefab 內找到 Image 組件，並透過 URL 載入圖片
        Image imageComponent = newProjectPrefab.GetComponent<Image>();
        if (imageComponent != null)
        {
            StartCoroutine(LoadImage(imageUrl, imageComponent));
        }
        else
        {
            Debug.LogError("Prefab 沒有 Image 組件，請檢查 Prefab 結構");
        }
        MoveAddButtonToEnd();
    }
    void OnPrefabClicked(string imageUrl)
    {
        PlayerPrefs.SetString("SelectedImageURL", imageUrl);
        SceneManager.LoadScene(1);
    }

    IEnumerator LoadImage(string url, Image imageComponent)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageComponent.sprite = sprite;
            }
            else
            {
                Debug.LogError("圖片載入失敗：" + request.error);
            }
        }
    }
    void MoveAddButtonToEnd()
    {
        // 找到加號按鈕
        Transform addButton = content.Find("Image"); // 確保加號按鈕的名稱是 "AddButton"

        if (addButton != null)
        {
            addButton.SetAsLastSibling(); // 移動到最後一個
        }
    }

}
