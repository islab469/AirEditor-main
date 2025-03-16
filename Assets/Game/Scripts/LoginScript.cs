using UnityEngine;
using TMPro;

public class LoginScript : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputEmail; // 連接到電子郵件輸入欄

    [SerializeField]
    TMP_InputField inputPassword; // 連接到密碼輸入欄

    [SerializeField]
    TextMeshProUGUI textEmail;

    [SerializeField]
    GameObject loginPanel;

    [SerializeField]
    bool debugMode = false;
    string userEmail = FirebaseManager.getEmail();

    // 註冊用戶的方法
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
        await FirebaseManager.Register(inputEmail.text, inputPassword.text); // 調用 FirebaseManager 的註冊方法
        updateEmailAndPassword();
    }

    // 登入用戶的方法
    public async void Login()
    {
        FirebaseManager.checkAndStart();
        await FirebaseManager.Login(inputEmail.text, inputPassword.text); // 調用 FirebaseManager 的登入方法
        updateEmailAndPassword();
        if (FirebaseManager.isLogin()){
            loginPanel.SetActive(false);
        }









    }
    // 獲取當前用戶的電子郵件
    

    // 登出用戶的方法
    public void Logout(){
        FirebaseManager.checkAndStart();
        FirebaseManager.Logout(); // 調用 FirebaseManager 的登出方法
    }

    private void updateEmailAndPassword(){
        FirebaseManager.email = inputEmail.text;
        FirebaseManager.password = inputPassword.text;
    }
}
