using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchPage : MonoBehaviour
{
    public static Page currentPage = Page.NONE;

    public enum Page
    {
        NONE,
        LOGIN,
        CREATE_PROJECT,
        PROJECT_INTERFACE,
        QA_INTERFACE,
        QS_INTERFACE,
        EDIT_QUESTION,
        Lobby,
        AIQA,
        LoginTransition
    }

    public void SwitchToLobby()
    {
        Debug.Log("Switching to Lobby scene");
        currentPage = Page.Lobby;
        SceneManager.LoadScene(0);
    }

    public void SwitchToCreateProject()
    {
        Debug.Log("Switching to Create Project scene");
        currentPage = Page.CREATE_PROJECT;
        SceneManager.LoadScene(1);
    }

    public void SwitchToProjectInterface()
    {
        Debug.Log("Switching to Project Interface scene");
        currentPage = Page.PROJECT_INTERFACE;
        SceneManager.LoadScene(2);
    }

    public void SwitchToQAInterface()
    {
        Debug.Log("Switching to QA Interface scene");
        currentPage = Page.QA_INTERFACE;
        SceneManager.LoadScene(3);
    }

    public void SwitchToQSInterface()
    {
        Debug.Log("Switching to QS Interface scene");
        currentPage = Page.QS_INTERFACE;
        SceneManager.LoadScene(4);
    }

    public void SwitchToEditQuestion()
    {
        Debug.Log("Switching to Edit Question scene");
        currentPage = Page.EDIT_QUESTION;
        SceneManager.LoadScene(5);
    }

    public void SwitchToLogin()
    {
        Debug.Log("Switching to Login scene");
        currentPage = Page.LOGIN;
        SceneManager.LoadScene(6);
    }

    public void SwitchToAIQA()
    {
        Debug.Log("Switching to AIQA scene");
        currentPage = Page.AIQA;
        SceneManager.LoadScene(8);
    }

    public void SwitchToLoginTransitionn()
    {
        Debug.Log("Switching to LoginTransition scene");
        currentPage = Page.LoginTransition;
        SceneManager.LoadScene(8);
    }
    private void Start()
    {
        Debug.Log($"Current Page: {currentPage}");
    }
}
