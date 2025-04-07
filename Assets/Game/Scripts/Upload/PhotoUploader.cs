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
}

