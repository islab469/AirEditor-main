using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchPage : MonoBehaviour
{
    public static Page curnPage = Page.LENGTH;

    public enum Page
    {
        MODEL,
        LENGTH
    }

    public void SwitchToCreateProject()
    {
        Debug.Log("🔁 Switching to Create Project scene");
        curnPage = Page.LENGTH;
        SceneSystem.changeScene(SceneType.SCENE_CREATE_PROJECT);
    }

    public void SwitchToUploadImage()
    {
        Debug.Log("🖼️ Switching to Upload Image scene");
        curnPage = Page.LENGTH;
        SceneSystem.changeScene(SceneType.SCENE_CHOOSE_IMAGE);
    }

    public void SwitchToLogin()
    {
        Debug.Log("🔐 Switching to Login scene");
        curnPage = Page.LENGTH;
        SceneManager.LoadScene(0);
    }

    public void SwitchToChooseImage()
    {
        Debug.Log("📂 Switching to Choose Image scene");
        curnPage = Page.LENGTH;
        SceneManager.LoadScene(1);
    }

    public void SwitchToAIQA()
    {
        Debug.Log("🤖 Switching to AI QA scene");
        curnPage = Page.LENGTH;
        SceneManager.LoadScene(2);
    }

    public void SwitchToProjectInterface()
    {
        Debug.Log("📁 Switching to Project Interface scene");
        curnPage = Page.LENGTH;
        SceneManager.LoadScene(3);
    }

    public void SwitchToQAInterface()
    {
        Debug.Log("📝 Switching to QA Interface scene");
        curnPage = Page.LENGTH;
        SceneManager.LoadScene(4);
    }

    public void SwitchToEditQuestion()
    {
        Debug.Log("✏️ Switching to Edit Question scene");
        SceneManager.LoadScene(6);
    }

    private void Start()
    {
        Debug.Log($"🚩 Current Page: {curnPage}");
        // 不需要執行模型相關邏輯
    }
}
