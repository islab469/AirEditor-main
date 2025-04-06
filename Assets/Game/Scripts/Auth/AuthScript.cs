using UnityEngine;
using TMPro;
using Firebase.Auth;
using System.Threading.Tasks;
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

    // 登入按鈕被點擊時
    public async void OnLoginClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        string result = await Login(email, password);

        switch (result)
        {
            case "SUCCESS":
                statusText.color = successColor;
                statusText.text = "Login successful.";
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
                statusText.text = "Account not registered.";
                break;
            default:
                statusText.color = errorColor;
                statusText.text = "Login failed.";
                break;
        }
    }

    // 註冊按鈕被點擊時
    public async void OnRegisterClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        string result = await Register(email, password);

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
                statusText.text = "Password too weak.";
                break;
            case "INTERNAL_ERROR":
                statusText.color = errorColor;
                statusText.text = "Account not registered.";
                break;
            default:
                statusText.color = errorColor;
                statusText.text = "Registration failed.";
                break;
        }
    }

    // 登出按鈕被點擊時
    public void OnLogoutClicked()
    {
        FirebaseManager.Logout();
        statusText.color = successColor;
        statusText.text = "You have successfully logged out.";
    }

    // 登入功能
    private async Task<string> Login(string email, string password)
    {
        try
        {
            var authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;
            Debug.Log($"Login successful: {user.Email}");
            return "SUCCESS";
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Login failed with error code: {ex.ErrorCode}, message: {ex.Message}");

            if (ex.ErrorCode == (int)AuthError.WrongPassword)
                return "INVALID_PASSWORD";
            else if (ex.ErrorCode == (int)AuthError.UserNotFound)
                return "EMAIL_NOT_FOUND";
            else if (ex.ErrorCode == (int)AuthError.InvalidEmail)
                return "INVALID_EMAIL";
            else
                return "INTERNAL_ERROR";
        }
        catch (System.Exception e)
        {
            Debug.LogError("Unexpected login error: " + e.Message);
            return "FAILURE";
        }
    }

    // 註冊功能
    private async Task<string> Register(string email, string password)
    {
        try
        {
            var authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;
            Debug.Log($"Registration successful: {user.Email}");
            return "REGISTER_SUCCESS";
        }
        catch (FirebaseException ex)
        {
            AuthError errorCode = (AuthError)ex.ErrorCode;
            Debug.LogError($"Register failed with error: {errorCode}");

            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    return "EMAIL_IN_USE";
                case AuthError.InvalidEmail:
                    return "INVALID_EMAIL";
                case AuthError.WeakPassword:
                    return "WEAK_PASSWORD";
                default:
                    return "INTERNAL_ERROR";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Unexpected registration error: " + e.Message);
            return "FAILURE";
        }
    }
}
