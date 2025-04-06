using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI statusText;

    public GameObject loginPanel;      // 登入面板
    public GameObject selectionPanel;  // 成功登入後的選單面板

    private readonly Color successColor = new Color(0.1f, 0.6f, 0.2f); // 綠色
    private readonly Color errorColor = Color.red;                     // 紅色
    private readonly Color warningColor = Color.blue;                 // 警告用藍色

    // 當按下 Login 按鈕時
    public async void OnLoginClicked()
{
    string email = emailInput.text.Trim();
    string password = passwordInput.text.Trim();

    // 基本輸入檢查
    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
    {
        statusText.color = errorColor;
        statusText.text = "Please enter both email and password.";
        return;
    }

    string result = await FirebaseManager.Login(email, password);

    switch (result)
    {
        case "SUCCESS":
            statusText.color = successColor;
            statusText.text = "Login successful.";
            ShowSelection();
            break;

        case "INVALID_PASSWORD":
            statusText.color = errorColor;
            statusText.text = "Wrong password.";
            break;

        case "EMAIL_NOT_FOUND":
            statusText.color = errorColor;
            statusText.text = "Account does not exist.";
            break;

        case "INVALID_EMAIL":
            statusText.color = errorColor;
            statusText.text = "Invalid email format.";
            break;

        case "USER_DISABLED":
            statusText.color = warningColor;
            statusText.text = "This account has been disabled.";
            break;

        case "FIREBASE_NOT_INITIALIZED":
            statusText.color = errorColor;
            statusText.text = "Authentication service not ready. Please try again.";
            break;

        default:
            statusText.color = errorColor;
            statusText.text = "Login failed. Please check your connection and try again.";
            Debug.LogError("Login failed: " + result);
            break;
    }
}

    // 當按下 Register（註冊）按鈕時
    public async void OnRegisterClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        string result = await FirebaseManager.Register(email, password);

        switch (result)
        {
            case "success":
                statusText.color = successColor;
                statusText.text = "Registration successful.";
                Debug.Log("Registration successful.");
                break;

            case "email-already-in-use":
                statusText.color = warningColor;
                statusText.text = "This email is already registered.";
                Debug.Log("Registration failed: Email already in use.");
                break;

            case "invalid-email":
                statusText.color = errorColor;
                statusText.text = "Invalid email format.";
                Debug.Log("Registration failed: Invalid email format.");
                break;

            case "weak-password":
                statusText.color = errorColor;
                statusText.text = "Password is too weak (at least 6 characters).";
                Debug.Log("Registration failed: Weak password.");
                break;

            default:
                statusText.color = errorColor;
                statusText.text = "Registration failed: " + result;
                Debug.Log("Registration failed: " + result);
                break;
        }
    }

    // 顯示選單面板並隱藏登入面板
    private void ShowSelection()
    {
        if (loginPanel != null)
            loginPanel.SetActive(false);
        if (selectionPanel != null)
            selectionPanel.SetActive(true);
    }
}
