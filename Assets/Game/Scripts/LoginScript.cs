using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;



public class LoginScript : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputEmail; // �s����q�l�l���J��

    [SerializeField]
    TMP_InputField inputPassword; // �s����K�X��J��

    [SerializeField]
    TextMeshProUGUI textEmail;

    [SerializeField]
    GameObject loginPanel;

    [SerializeField]
    bool debugMode = false;
    string userEmail = FirebaseManager.getEmail();

    // ���U�Τ᪺��k
    private void Start()
    {

        QualitySettings.vSyncCount = 0;
        UnityEngine.Application.targetFrameRate = 30;
        if (FirebaseManager.isLogin()){
            loginPanel.SetActive(false);
        }

        if (debugMode) {
            inputEmail.text = "asd123@gmail.com";
            inputPassword.text = "123456789";

        }

        RequestManager.sendTestRequestToFetchQuestion();
        //RequestManager.sendTestRequestToGeneateQuestion();
    }
    public async void Register(){
        FirebaseManager.checkAndStart();
        await FirebaseManager.Register(inputEmail.text, inputPassword.text); // �ե� FirebaseManager �����U��k
        updateEmailAndPassword();
    }

    // �n�J�Τ᪺��k
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private Text errorMessage;

    public async void Login()
    {
        //loadingIndicator.SetActive(true); // ��� Loading �ʵe
        //errorMessage.text = ""; // �M�ſ��~�T��
        FirebaseManager.checkAndStart();
        try
        {
            await FirebaseManager.Login(inputEmail.text, inputPassword.text);
            if (FirebaseManager.isLogin())
            {
                Debug.LogError("�n�Jsuccess");
                loginPanel.SetActive(false);
                updateEmailAndPassword();
            }
            else
            {
                errorMessage.text = "�n�J���ѡA���ˬd�b���K�X�I";
                Debug.LogError("�n�J����");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"�n�J���~: {e.Message}");

        }
        finally
        {
            //loadingIndicator.SetActive(false); // ���� Loading �ʵe
        }
        /*FirebaseManager.checkAndStart();
        await FirebaseManager.Login(inputEmail.text, inputPassword.text); // �ե� FirebaseManager ���n�J��k
        updateEmailAndPassword();
        if (FirebaseManager.isLogin()){
            loginPanel.SetActive(false);
        }*/
    }
    // �����e�Τ᪺�q�l�l��
    
    // �n�X�Τ᪺��k
    public void Logout(){
        FirebaseManager.checkAndStart();
        FirebaseManager.Logout(); // �ե� FirebaseManager ���n�X��k
    }

    private void updateEmailAndPassword(){
        FirebaseManager.email = inputEmail.text;
        FirebaseManager.password = inputPassword.text;
    }
}
