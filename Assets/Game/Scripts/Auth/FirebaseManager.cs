using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine;

public static class FirebaseManager
{
    private static FirebaseAuth auth;
    private static FirebaseUser user;

    // 初始化 Firebase Auth 實例
    static FirebaseManager()
    {
        InitializeFirebase();
    }

    private static void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    // 登入功能
    public static async Task<string> Login(string email, string password)
    {
        try
        {
            var authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            user = authResult.User; // 這裡回傳的就是 FirebaseUser
            Debug.Log($"Login successful: {user.Email}");
            return "SUCCESS";
        }
        catch (FirebaseException ex)
        {
            AuthError errorCode = (AuthError)ex.ErrorCode;
            Debug.LogError($"Login failed with error: {errorCode}");

            switch (errorCode)
            {
                case AuthError.InvalidEmail:
                    return "INVALID_EMAIL";
                case AuthError.UserNotFound:
                    return "EMAIL_NOT_FOUND";
                case AuthError.WrongPassword:
                    return "INVALID_PASSWORD";
                case AuthError.UserDisabled:
                    return "USER_DISABLED";
                default:
                    return "FAILURE";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Unexpected login error: " + e.Message);
            return "FAILURE";
        }
    }

    // 註冊功能
    public static async Task<string> Register(string email, string password)
    {
        try
        {
            var authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            user = authResult.User;
            Debug.Log($"Registration successful: {user.Email}");
            return "success";
        }
        catch (FirebaseException ex)
        {
            AuthError errorCode = (AuthError)ex.ErrorCode;
            Debug.LogError($"Register failed with error: {errorCode}");

            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    return "email-already-in-use";
                case AuthError.InvalidEmail:
                    return "invalid-email";
                case AuthError.WeakPassword:
                    return "weak-password";
                default:
                    return "failure";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Unexpected registration error: " + e.Message);
            return "failure";
        }
    }

    // 取得使用者 Email
    public static string GetEmail()
    {
        return user != null ? user.Email : null;
    }

    // 取得完整使用者資訊
    public static FirebaseUser GetUser()
    {
        return user;
    }

    // 登出
    public static void Logout()
    {
        auth.SignOut();
        user = null;
        Debug.Log("User signed out.");
    }
}
