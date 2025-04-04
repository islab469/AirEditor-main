using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;



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
        Application.targetFrameRate = 30;

        if (FirebaseManager.isLogin())
        {
            loginPanel.SetActive(false);
        }

        if (debugMode)
        {
            inputEmail.text = "asd123@gmail.com";
            inputPassword.text = "123456789";
        }

        // ✅ 呼叫你的 async 方法（不會報警告）
        _ = FetchTestQuestions();
    }

    // ✅ 這是你定義的 async 方法
    private async Task FetchTestQuestions()
    {
        await RequestManager.sendTestRequestToFetchQuestion();
    }

    public async void Register(){
        FirebaseManager.checkAndStart();
        await FirebaseManager.Register(inputEmail.text, inputPassword.text); // 調用 FirebaseManager 的註冊方法
        updateEmailAndPassword();
    }

    // 登入用戶的方法
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private Text errorMessage;

    public async void Login()
    {
        //loadingIndicator.SetActive(true); // 顯示 Loading 動畫
        //errorMessage.text = ""; // 清空錯誤訊息
        FirebaseManager.checkAndStart();
        try
        {
            await FirebaseManager.Login(inputEmail.text, inputPassword.text);
            if (FirebaseManager.isLogin())
            {
                Debug.LogError("登入success");
                loginPanel.SetActive(false);
                updateEmailAndPassword();
            }
            else
            {
                errorMessage.text = "登入失敗，請檢查帳號密碼！";
                Debug.LogError("登入失敗");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"登入錯誤: {e.Message}");

        }
        finally
        {
            //loadingIndicator.SetActive(false); // 隱藏 Loading 動畫
        }
        /*FirebaseManager.checkAndStart();
        await FirebaseManager.Login(inputEmail.text, inputPassword.text); // 調用 FirebaseManager 的登入方法
        updateEmailAndPassword();
        if (FirebaseManager.isLogin()){
            loginPanel.SetActive(false);
        }*/
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
