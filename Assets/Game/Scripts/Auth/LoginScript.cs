using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI statusText;

    private readonly Color successColor = new Color(0.1f, 0.6f, 0.2f); // 綠色
    private readonly Color errorColor = Color.red; // 紅色

    // 當按下 Login 按鈕時
    public async void OnLoginClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        string errorMessage = await FirebaseManager.Login(email, password);
        if (errorMessage == "success")
        {
            statusText.text = "Login successful.";
            statusText.color = Color.green;
        }
        else
        {
            statusText.text = errorMessage;
            statusText.color = Color.red;
        }
    }

    public async void OnRegisterClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        string result = await FirebaseManager.Register(email, password);
        if (result == "success")
        {
            statusText.text = "Registration successful.";
            statusText.color = Color.green;
        }
        else if (result == "email-already-in-use")
        {
            statusText.text = "This email is already registered.";
            statusText.color = Color.blue;
        }
        else
        {
            statusText.text = result;
            statusText.color = Color.red;
        }
    }

}
