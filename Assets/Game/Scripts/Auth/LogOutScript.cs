using UnityEngine;

public class LogOutScript : MonoBehaviour
{
    // �n�X�í��ҵn�J�e��
    public void OnLogout()
    {
        FirebaseManager.Logout();
        Debug.Log("User logged out.");
    }
}
