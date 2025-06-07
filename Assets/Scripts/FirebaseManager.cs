using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using Firebase;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();

            if (auth != null)
                auth.StateChanged += AuthStateChanged;
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
            Debug.LogError("❌ Firebase initialization failed.");
            return;
        }

        Debug.Log("✅ Firebase initialized.");
    }

    public static async Task<string> Register(string email, string password)
    {
        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log($"✅ Registered: {email}");
            auth.SignOut(); // 登記後自動登出
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
                _ => "INTERNAL_ERROR"
            };
        }
    }

    public static async Task<string> Login(string email, string password)
    {
        try
        {
            var authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            user = authResult.User;
            Debug.Log($"✅ Login: {user.Email}");
            return "SUCCESS";
        }
        catch (FirebaseException ex)
        {
            AuthError errorCode = (AuthError)ex.ErrorCode;
            Debug.LogError($"Login error: {errorCode}");
            return errorCode switch
            {
                AuthError.UserNotFound => "EMAIL_NOT_FOUND",
                AuthError.WrongPassword => "INVALID_PASSWORD",
                AuthError.InvalidEmail => "INVALID_EMAIL",
                _ => "INTERNAL_ERROR"
            };
        }
    }

    public static void Logout()
    {
        if (auth != null)
        {
            auth.SignOut();
            user = null;
            Debug.Log("👤 User signed out.");
        }

        SceneManager.LoadScene(6); // 回到登入畫面
    }

    private static void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            user = auth.CurrentUser;

            if (user != null)
                Debug.Log("👤 User signed in: " + user.Email);
            else
                Debug.Log("👤 User signed out.");
        }
    }

    public static string GetEmail() => user?.Email; // ✅ 保留
    public static bool IsLoggedIn() => auth != null && auth.CurrentUser != null;
}
