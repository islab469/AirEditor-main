using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SFB;

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
            PlayerPrefs.SetString("UploadedImagePath", paths[0]);
            SceneManager.LoadScene(3);
        }
    }

    //IEnumerator LoadImages(string path)
    //{ 


    //    if (File.Exists(path))
    //    {
    //        byte[] imageData = File.ReadAllBytes(path);
    //        Texture2D tex = new Texture2D(2, 2);
    //        tex.LoadImage(imageData);

    //        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    //        if (sprite == null) {
    //            Debug.Log("Sprite is null");
    //        }
    //        DontDestroyOnLoad(sprite);
    //        SceneManager.LoadScene(3);
    //        imageManager.SetSpite(sprite);
            
    //        yield return null;
    //    }


    ////}
    
}

