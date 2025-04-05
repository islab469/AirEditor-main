using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI statusText;

    // 當按下 Login 按鈕時
    public async void OnLoginClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (await FirebaseManager.Login(email, password))
        {
            statusText.text = "Login successful.";
        }
        else
        {
            statusText.text = "Login failed. Check credentials.";
        }
    }

    // 當按下 Register 按鈕時
    public async void OnRegisterClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (await FirebaseManager.Register(email, password))
        {
            statusText.text = "Registration successful.";
        }
        else
        {
            statusText.text = "Registration failed.";
        }
    }
}
