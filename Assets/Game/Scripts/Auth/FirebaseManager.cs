using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using Firebase;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance
    {
        get; private set;
    }
    public static FirebaseAuth auth;
    public static FirebaseFirestore firestore;
    public static FirebaseUser user;
    public static DatabaseReference databaseReference;

    public GameObject PanelLogin;
    public GameObject PanelSelection;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();

            // 只有在這裡掛上狀態監聽，避免註冊時提前切換畫面
            if (auth != null)
            {
                auth.StateChanged += AuthStateChanged;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void InitializeFirebase()
    {
        if (auth != null)
            return;

        auth = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;

        if (auth == null || firestore == null)
        {
            Debug.LogError("Firebase initialization failed.");
            return;
        }

        Debug.Log("Firebase initialized successfully.");
    }

    // 註冊
    public static async Task<string> Register(string email, string password)
    {
        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log($"Registration successful for: {email}");

            auth.SignOut(); // 註冊後直接登出
            return "REGISTER_SUCCESS";
        }
        catch (FirebaseException ex)
        {
            var errorCode = (AuthError)ex.ErrorCode;
            Debug.LogError($"Register failed: {errorCode}");
            return errorCode switch
            {
                AuthError.EmailAlreadyInUse => "EMAIL_IN_USE",
                AuthError.InvalidEmail => "INVALID_EMAIL",
                AuthError.WeakPassword => "WEAK_PASSWORD",
                _ => "REGISTER_FAILURE"
            };
        }
    }

    // 登入
    public static async Task<string> Login(string email, string password)
    {
        try
        {
            var authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            user = authResult.User;
            Debug.Log($"Login successful: {user.Email}");
            return "SUCCESS";
        }
        catch (FirebaseException ex)
        {
            AuthError errorCode = (AuthError)ex.ErrorCode;
            Debug.LogError($"Login failed: Firebase error code {ex.ErrorCode}, message: {ex.Message}, stack: {ex.StackTrace}");

            switch (errorCode)
            {
                case AuthError.UserNotFound:
                    return "EMAIL_NOT_FOUND";
                case AuthError.WrongPassword:
                    return "INVALID_PASSWORD";
                case AuthError.InvalidEmail:
                    return "INVALID_EMAIL";
                case AuthError.Failure: // 處理通用的 Failure 錯誤
                    return "EMAIL_NOT_FOUND"; // 假設這表示帳號未註冊
                default:
                    Debug.LogError($"Unhandled Firebase error code: {errorCode}");
                    return "INTERNAL_ERROR";
            }
        }
    }

    // 登出
    public static void Logout()
    {
        if (auth != null)
        {
            auth.SignOut();
            user = null;
            Debug.Log("User signed out.");
        }
        // 確保登出後即時更新 UI
        Instance.PanelLogin.SetActive(true);
        Instance.PanelSelection.SetActive(false);
    }

    private static void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            user = auth.CurrentUser;

            if (user != null)
            {
                Instance.PanelLogin?.SetActive(false);
                Instance.PanelSelection?.SetActive(true);
                Debug.Log("User signed in: " + user.Email);
            }
            else
            {
                Instance.PanelLogin?.SetActive(true);
                Instance.PanelSelection?.SetActive(false);
                Debug.Log("User signed out.");
            }
        }
    }

    public static string GetEmail() => user?.Email;
    public static bool IsLoggedIn() => auth != null && auth.CurrentUser != null;
}
