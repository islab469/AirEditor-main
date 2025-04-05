using UnityEngine;

public class LogOutScript : MonoBehaviour
{
    // 登出並重啟登入畫面
    public void OnLogout()
    {
        FirebaseManager.Logout();
        Debug.Log("User logged out.");
    }
}
