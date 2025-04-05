using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// 上傳圖片與使用者資料至伺服器的處理類別
/// </summary>
public class ImageUploader : MonoBehaviour
{
    private ImageManager imageManager;
    private string imageName;

    private void Awake()
    {
        // 初始化 imageManager，確保元件存在於場景中
        imageManager = FindObjectOfType<ImageManager>();
        if (imageManager == null)
        {
            Debug.LogError("ImageManager not found. Please make sure it exists in the scene.");
        }
    }

    /// <summary>
    /// 呼叫此函式以開始上傳圖片與資訊
    /// </summary>
    public void Upload()
    {
        imageName = PlayerPrefs.GetString("SelectedModel", "Dog");
        string urlText = URLsave.url;

        if (string.IsNullOrEmpty(urlText))
        {
            Debug.LogError("URL is empty. Please enter a valid URL.");
            return;
        }

        string email = FirebaseManager.GetEmail();
        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("Email not available. Make sure user is logged in.");
            return;
        }

        byte[] imageData = GetSpriteBytes();
        if (imageData != null)
        {
            StartCoroutine(UploadFileCoroutine(imageData, urlText, email));
        }
    }

    private byte[] GetSpriteBytes()
    {
        Sprite sprite = imageManager.GetCurrentSprite();
        if (sprite == null)
        {
            Debug.LogError("No sprite available for upload.");
            return null;
        }

        Texture2D texture = sprite.texture;
        Texture2D readableTex = DeCompress(texture);
        return readableTex.EncodeToPNG();
    }

    private IEnumerator UploadFileCoroutine(byte[] imageData, string urlText, string email)
    {
        string apiUrl = "http://127.0.0.1:8000/unitydata/upload_data/";
        WWWForm form = new WWWForm();

        form.AddBinaryData("image", imageData, imageName + ".png", "image/png");
        form.AddField("url", urlText);
        form.AddField("email", email);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Upload failed: " + www.error + "\nServer Response: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Upload successful! Server Response: " + www.downloadHandler.text);
            }
        }
    }

    public static Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;

        Texture2D readableTex = new Texture2D(source.width, source.height);
        readableTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);

        return readableTex;
    }
}
