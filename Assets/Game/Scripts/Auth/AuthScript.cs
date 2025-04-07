using UnityEngine;
using TMPro;
using Firebase.Auth;
using System.Threading.Tasks;

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

    public async void OnLoginClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        string result = await FirebaseManager.Login(email, password);

        switch (result)
        {
            case "SUCCESS":
                statusText.color = successColor;
                statusText.text = "✅ Login successful.";
                break;
            case "EMAIL_NOT_FOUND":
                statusText.color = errorColor;
                statusText.text = "⚠️ Account not registered.";
                break;
            case "INVALID_PASSWORD":
                statusText.color = errorColor;
                statusText.text = "❌ Incorrect password.";
                break;
            case "INVALID_EMAIL":
                statusText.color = errorColor;
                statusText.text = "❌ Invalid email format.";
                break;
            case "INTERNAL_ERROR":
                statusText.color = errorColor;
                statusText.text = "⚠️ Account not registered.";
                break;
            default:
                statusText.color = errorColor;
                statusText.text = "❌ Login failed.";
                break;
        }
    }

    public async void OnRegisterClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        string result = await FirebaseManager.Register(email, password);

        switch (result)
        {
            case "REGISTER_SUCCESS":
                statusText.color = successColor;
                statusText.text = "✅ Registration successful. Please login.";
                break;
            case "EMAIL_IN_USE":
                statusText.color = errorColor;
                statusText.text = "⚠️ Email already in use.";
                break;
            case "INVALID_EMAIL":
                statusText.color = errorColor;
                statusText.text = "❌ Invalid email format.";
                break;
            case "WEAK_PASSWORD":
                statusText.color = errorColor;
                statusText.text = "⚠️ Password too weak.";
                break;
            case "INTERNAL_ERROR":
                statusText.color = errorColor;
                statusText.text = "⚠️ Server error.";
                break;
            default:
                statusText.color = errorColor;
                statusText.text = "❌ Registration failed.";
                break;
        }
    }

    public void OnLogoutClicked()
    {
        FirebaseManager.Logout();
        statusText.color = successColor;
        statusText.text = "✅ You have logged out.";
    }
}
