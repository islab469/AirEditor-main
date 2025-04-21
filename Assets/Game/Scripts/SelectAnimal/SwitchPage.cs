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
        LoginTransition,
        AR
    }

    public void SwitchToLobby()
    {
        Debug.Log("Switching to Lobby scene");
        currentPage = Page.Lobby;
        SceneManager.LoadScene("Lobby");
    }

    public void SwitchToCreateProject()
    {
        Debug.Log("Switching to Create Project scene");
        currentPage = Page.CREATE_PROJECT;
        SceneManager.LoadScene("CreateProject");
    }

    public void SwitchToProjectInterface()
    {
        Debug.Log("Switching to Project Interface scene");
        currentPage = Page.PROJECT_INTERFACE;
        SceneManager.LoadScene("ProjectInterface");
    }

    public void SwitchToQAInterface()
    {
        Debug.Log("Switching to QA Interface scene");
        currentPage = Page.QA_INTERFACE;
        SceneManager.LoadScene("QAInterface");
    }

    public void SwitchToQSInterface()
    {
        Debug.Log("Switching to QS Interface scene");
        currentPage = Page.QS_INTERFACE;
        SceneManager.LoadScene("QSInterface");
    }

    public void SwitchToEditQuestion()
    {
        Debug.Log("Switching to Edit Question scene");
        currentPage = Page.EDIT_QUESTION;
        SceneManager.LoadScene("EditQuestion");
    }

    public void SwitchToLogin()
    {
        Debug.Log("Switching to Login scene");
        currentPage = Page.LOGIN;
        SceneManager.LoadScene("Login");
    }

    public void SwitchToAIQA()
    {
        Debug.Log("Switching to AIQA scene");
        currentPage = Page.AIQA;
        SceneManager.LoadScene("AIQA");
    }

    public void SwitchToLoginTransitionn()
    {
        Debug.Log("Switching to LoginTransition scene");
        currentPage = Page.LoginTransition;
        SceneManager.LoadScene("LoginTransition");
    }

    public void SwitchToAR()
    {
        Debug.Log("Switching to AR scene");
        currentPage = Page.AR;
        SceneManager.LoadScene("ARTEST");
    }

    private void Start()
    {
        Debug.Log($"Current Page: {currentPage}");
    }
}
