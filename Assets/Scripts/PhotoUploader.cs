using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SFB;
using System.Collections;
using System.IO;

public class PhotoUploader : MonoBehaviour
{
    public Button uploadButton;
    public Image image;

    void Start()
    {
        if (uploadButton == null)
        {
            Debug.LogError("UploadButton is not assigned in the inspector.");
            return;
        }

        if (image == null)
        {
            Debug.LogError("Image is not assigned in the inspector.");
            return;
        }

        uploadButton.onClick.AddListener(OnUploadButtonClick);
    }

    public void OnUploadButtonClick()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Images", "", extensions, false);

        if (paths.Length > 0)
        {
            string path = paths[0];
            PlayerPrefs.SetString("UploadedImagePath", path);
            StartCoroutine(LoadImage(path)); // ✅ 加這行來載入圖片
        }
    }

    private IEnumerator LoadImage(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Selected image file does not exist.");
            yield break;
        }

        byte[] imageData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        image.sprite = sprite;
        yield return null;
    }
}
