using UnityEngine;
using TMPro;

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
    public async void Login()
    {
        FirebaseManager.checkAndStart();
        await FirebaseManager.Login(inputEmail.text, inputPassword.text); // �ե� FirebaseManager ���n�J��k
        updateEmailAndPassword();
        if (FirebaseManager.isLogin()){
            loginPanel.SetActive(false);
        }









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
