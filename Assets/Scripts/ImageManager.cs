using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading.Tasks;
using System;


public enum Animal
{
    ANIMAL_CAT,
    ANIMAL_CHICKEN,
    ANIMAL_DEER,
    ANIMAL_DOG,
    ANIMAL_ELEPHANT,
    ANIMAL_HORSE,
    LENGTH
}
class ImageManager : MonoBehaviour
{
    private Sprite currentSprite; // 保存當前使用的圖片


    [SerializeField]
    private Image modelImage; // 空的 Image 組件，用於顯示選擇的圖片


    private void Start()
    {

        UpdateModelImage();
    }

    private int lastModelID = -1;

    private void Update()
    {
        int currentModelID = PlayerPrefs.GetInt("SelectedModel", 0);
        if (currentModelID != lastModelID)
        {
            lastModelID = currentModelID;
            UpdateModelImage();
        }
    }
    private void UpdateModelImage()
    {
        Animal selectedModel = (Animal)PlayerPrefs.GetInt("SelectedModel", 0); // 默認為狗模型
        Debug.Log($"【從 PlayerPrefs 獲取選擇的動物】{selectedModel}");
        Sprite sprite = AnimalUtil.GetAnimalSpriteByAnimalID((int)selectedModel);
        modelImage.sprite = sprite;
        currentSprite = sprite;
    }

    public void UploadImage()
    {
        string path = "你的圖片路徑";
        PlayerPrefs.SetString("UploadedImagePath", path);
        PlayerPrefs.Save();

        Debug.Log($"【上傳圖片】路徑：{path}");

        UpdateModelImage(); // 上傳後立即更新
    }

    public Sprite GetCurrentSprite()
    {
        if (currentSprite == null)
        {
            Debug.LogError("【GetCurrentSprite】當前沒有可用的圖片！");
        }
        return currentSprite;
    }
}

