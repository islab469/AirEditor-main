using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class UploadFile : MonoBehaviour
{
    private ImageManager imageManager;
    private string imageName;

    private Sprite currentSprite;
    

    private void Awake()
    {
        // ��l�� imageManager�A�T�O�b���s�I���ɴN�w�g�ǳƦn
        imageManager = FindObjectOfType<ImageManager>();
        if (imageManager == null)
        {
            Debug.LogError("ImageManager �䤣��A�нT�O���w�g�s�b�������");
        }
    }

    public void Upload()
    {
        // �q PlayerPrefs ���o imageName
        imageName = PlayerPrefs.GetString("SelectedModel", "Dog");

        // Get url from the input object
        string URLText = URLsave.url;

        if (string.IsNullOrEmpty(URLText))
        {
            Debug.LogError("URLText ���šA�Х���J URL");
            return;
        }
        string email = FirebaseManager.getEmail();
        // �}�l�W��
        StartCoroutine(Uploadfile(GetSprite(), URLText,email));
    }

    private byte[] GetSprite()
    {
        Sprite currentSprite = imageManager.GetCurrentSprite();
        if (currentSprite == null)
        {
            Debug.LogError("�L�k���o��e�� Sprite");
            return null;
        }

        Texture2D texture = currentSprite.texture;
        byte[] imageData = DeCompress(texture).EncodeToPNG();
        return imageData;
    }

    IEnumerator Uploadfile(byte[] imageData, string URLText,string email)
    {
        if (imageData == null)
        {
            Debug.LogError("�v���ƾڬ��šA�L�k�W��");
            yield break;
        }

        string url = "http://127.0.0.1:8000/unitydata/upload_data/";
        WWWForm form = new WWWForm();
        
        form.AddBinaryData("image", imageData, imageName + ".png", "image/png");
        form.AddField("url", URLText);
        form.AddField("email", email);
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error + "\nResponse: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Upload complete! Response: " + www.downloadHandler.text);
            }

        }
    }
    public static Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

}
