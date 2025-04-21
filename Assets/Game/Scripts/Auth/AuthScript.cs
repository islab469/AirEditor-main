using UnityEngine;
using TMPro;
using Firebase.Auth;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Firebase;

public class AuthScript : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI statusText;

    private readonly Color errorColor = Color.red;
    private readonly Color successColor = Color.green;

    private FirebaseAuth auth;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    // ✅ 登入功能
    public async void OnLoginClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        try
        {
            var result = await FirebaseManager.Login(email, password);

            switch (result)
            {
                case "SUCCESS":
                    statusText.color = successColor;
                    statusText.text = "登入成功。";
                    SceneManager.LoadScene(8);
                    break;

                case "EMAIL_NOT_FOUND":
                    statusText.color = errorColor;
                    statusText.text = "帳號未註冊。";
                    break;

                case "INVALID_PASSWORD":
                    statusText.color = errorColor;
                    statusText.text = "密碼錯誤。";
                    break;

                case "INVALID_EMAIL":
                    statusText.color = errorColor;
                    statusText.text = "電子郵件格式錯誤。";
                    break;

                case "INTERNAL_ERROR":
                    statusText.color = errorColor;
                    statusText.text = "內部錯誤，請稍後再試。";
                    break;

                default:
                    statusText.color = errorColor;
                    statusText.text = "登入失敗，請檢查您的帳號或密碼。";
                    break;
            }
        }
        catch (FirebaseException ex)
        {
            // 捕獲 Firebase 錯誤，並顯示詳細錯誤信息
            statusText.color = errorColor;
            statusText.text = "登入過程中出現錯誤: " + ex.Message;
        }
    }

    // ✅ 註冊功能（新增密碼強度檢查）
    public async void OnRegisterClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (!IsPasswordValid(password))
        {
            statusText.color = errorColor;
            statusText.text = "密碼必須至少包含 8 個字符，並且包含大寫字母、小寫字母、數字和特殊符號。";
            return;
        }

        try
        {
            var result = await FirebaseManager.Register(email, password);

            switch (result)
            {
                case "REGISTER_SUCCESS":
                    statusText.color = successColor;
                    statusText.text = "註冊成功，請進行登入。";
                    break;

                case "EMAIL_IN_USE":
                    statusText.color = errorColor;
                    statusText.text = "該電子郵件已經被註冊。";
                    break;

                case "INVALID_EMAIL":
                    statusText.color = errorColor;
                    statusText.text = "電子郵件格式錯誤。";
                    break;

                case "WEAK_PASSWORD":
                    statusText.color = errorColor;
                    statusText.text = "密碼強度不足，請使用較強的密碼。";
                    break;

                case "INTERNAL_ERROR":
                    statusText.color = errorColor;
                    statusText.text = "內部錯誤，請稍後再試。";
                    break;

                default:
                    statusText.color = errorColor;
                    statusText.text = "註冊失敗，請稍後再試。";
                    break;
            }
        }
        catch (FirebaseException ex)
        {
            // 捕獲 Firebase 錯誤，並顯示詳細錯誤信息
            statusText.color = errorColor;
            statusText.text = "註冊過程中出現錯誤: " + ex.Message;
        }
    }

    // ✅ 密碼格式檢查（使用正則表達式）
    private bool IsPasswordValid(string password)
    {
        if (password.Length < 8)
            return false;

        bool hasUpper = Regex.IsMatch(password, "[A-Z]");
        bool hasLower = Regex.IsMatch(password, "[a-z]");
        bool hasDigit = Regex.IsMatch(password, "[0-9]");
        bool hasSpecial = Regex.IsMatch(password, "[^a-zA-Z0-9]");

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }

    // ✅ 登出
    public void OnLogoutClicked()
    {
        FirebaseManager.Logout();
        statusText.color = successColor;
        statusText.text = "您已經成功登出。";
        SceneManager.LoadScene(6);
    }
}
