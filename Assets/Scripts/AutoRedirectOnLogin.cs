using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoRedirectOnLogin : MonoBehaviour
{
    void Start()
    {
        // ✅ 確保只在 LoginScene 才觸發自動跳轉
        if (SceneManager.GetActiveScene().buildIndex == 6 && FirebaseManager.IsLoggedIn())
        {
            Debug.Log("✅ Already logged in. Redirecting to Lobby...");
            SceneManager.LoadScene(0); // Lobby scene
        }
        else
        {
            Debug.Log("🔒 Not logged in or not in LoginScene. Stay here.");
        }
    }
}
