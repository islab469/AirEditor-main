using UnityEngine;
using TMPro;
using Firebase.Auth;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

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

        string result = await FirebaseManager.Login(email, password);

        switch (result)
        {
            case "SUCCESS":
                statusText.color = successColor;
                statusText.text = "Login successful.";
                SceneManager.LoadScene(8);
                break;

            case "EMAIL_NOT_FOUND":
                statusText.color = errorColor;
                statusText.text = "Account not registered.";
                break;

            case "INVALID_PASSWORD":
                statusText.color = errorColor;
                statusText.text = "Incorrect password.";
                break;

            case "INVALID_EMAIL":
                statusText.color = errorColor;
                statusText.text = "Invalid email format.";
                break;

            case "INTERNAL_ERROR":
                statusText.color = errorColor;
                statusText.text = "Incorrect password.";
                break;

            default:
                statusText.color = errorColor;
                statusText.text = "Login failed.";
                break;
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
            statusText.text = "Password must be at least 8 characters and include uppercase, lowercase, number, and special symbol.";
            return;
        }

        string result = await FirebaseManager.Register(email, password);

        switch (result)
        {
            case "REGISTER_SUCCESS":
                statusText.color = successColor;
                statusText.text = "Registration successful. Please login.";
                break;

            case "EMAIL_IN_USE":
                statusText.color = errorColor;
                statusText.text = "Email already in use.";
                break;

            case "INVALID_EMAIL":
                statusText.color = errorColor;
                statusText.text = "Invalid email format.";
                break;

            case "WEAK_PASSWORD":
                statusText.color = errorColor;
                statusText.text = "Password is too weak.";
                break;

            case "INTERNAL_ERROR":
                statusText.color = errorColor;
                statusText.text = "Incorrect password.";
                break;

            default:
                statusText.color = errorColor;
                statusText.text = "Registration failed.";
                break;
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
        statusText.text = "You have logged out.";
        SceneManager.LoadScene(6);
    }
}
