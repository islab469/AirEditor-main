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
                print("���n�ӤW�Ǫ��Ϥ����");
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        Dictionary<string, object> data = document.ToDictionary();
                        if (data.ContainsKey("image_url"))
                        {
                            string imageUrl = data["image_url"].ToString();
                            Debug.Log("���Ϥ� URL�G" + imageUrl);
                            CreatePrefab(imageUrl);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Firestore Ū�����ѡG" + task.Exception);
            }
        });

    }

    void CreatePrefab(string imageUrl)
    {
        GameObject newProjectPrefab = Instantiate(projectPrefab, content);
        Debug.Log("���\�ͦ� Prefab �å[�J ScrollView");
        Button button = newProjectPrefab.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnPrefabClicked(imageUrl));
        }

        // �b Prefab ����� Image �ե�A�óz�L URL ���J�Ϥ�
        Image imageComponent = newProjectPrefab.GetComponent<Image>();
        if (imageComponent != null)
        {
            StartCoroutine(LoadImage(imageUrl, imageComponent));
        }
        else
        {
            Debug.LogError("Prefab �S�� Image �ե�A���ˬd Prefab ���c");
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
                Debug.LogError("�Ϥ����J���ѡG" + request.error);
            }
        }
    }
    void MoveAddButtonToEnd()
    {
        // ���[�����s
        Transform addButton = content.Find("Image"); // �T�O�[�����s���W�٬O "AddButton"

        if (addButton != null)
        {
            addButton.SetAsLastSibling(); // ���ʨ�̫�@��
        }
    }

}
