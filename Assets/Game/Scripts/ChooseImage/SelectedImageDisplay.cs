using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class SelectedImageDisplay : MonoBehaviour
{
    public Image displayImage;

    void Start()
    {
        string url = PlayerPrefs.GetString("SelectedImageURL", "");
        Debug.Log("🎯 SelectedImageURL from PlayerPrefs: " + url);

        if (!string.IsNullOrEmpty(url))
        {
            StartCoroutine(LoadImageFromUrl(url));
        }
        else
        {
            Debug.LogWarning("❌ No URL found in PlayerPrefs.");
        }
    }

    IEnumerator LoadImageFromUrl(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                displayImage.sprite = sprite;
                Debug.Log("Displayed selected image.");
            }
            else
            {
                Debug.LogError("Failed to load image: " + request.error);
            }
        }
    }
}
