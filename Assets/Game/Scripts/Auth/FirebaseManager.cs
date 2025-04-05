using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using System.Threading.Tasks;
using Firebase.Extensions;
using FirestoreModels;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (PanelLogin != null) PanelLogin.SetActive(true);
        if (PanelSelection != null) PanelSelection.SetActive(false);
    }

    // 初始化 Firebase 認證與 Firestore
    public static void InitializeFirebase()
    {
        if (auth != null) return;

        auth = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;

        if (auth == null || firestore == null)
        {
            Debug.LogError("Firebase initialization failed.");
            return;
        }

        auth.StateChanged += AuthStateChanged;
        Debug.Log("Firebase initialized successfully.");
    }

    // 註冊新帳號
    public static async Task<bool> Register(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Register failed: Email or password is empty.");
            return false;
        }

        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            await WriteUserToFirestore(email, "New User");
            Debug.Log("User registered and stored in Firestore.");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Register failed: " + e.Message);
            return false;
        }
    }

    // 登入功能
    public static async Task<bool> Login(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Login failed: Email or password is empty.");
            return false;
        }

        try
        {
            await auth.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log("Login success: " + email);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Login failed: " + ex.Message);
            return false;
        }
    }

    // 登出功能
    public static void Logout()
    {
        auth.SignOut();
    }

    // 認證狀態監控
    private static void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            user = auth.CurrentUser;

            if (user != null)
            {
                if (Instance.PanelLogin != null) Instance.PanelLogin.SetActive(false);
                if (Instance.PanelSelection != null) Instance.PanelSelection.SetActive(true);
                Debug.Log("User signed in: " + user.Email);
            }
            else
            {
                if (Instance.PanelLogin != null) Instance.PanelLogin.SetActive(true);
                if (Instance.PanelSelection != null) Instance.PanelSelection.SetActive(false);
                Debug.Log("User signed out.");
            }
        }
    }

    // 寫入 Firestore 用戶資料
    public static async Task WriteUserToFirestore(string email, string displayName)
    {
        if (firestore == null)
        {
            Debug.LogError("Firestore is null. Cannot write data.");
            return;
        }

        var newUser = new UserModel(email, displayName);
        var docRef = firestore.Collection("users").Document(email);

        try
        {
            await docRef.SetAsync(newUser.GetDictionary());
            Debug.Log("User data written to Firestore.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Firestore write failed: " + ex.Message);
        }
    }

    // 取得當前登入者的 email
    public static string GetEmail()
    {
        return user != null ? user.Email : null;
    }

    // 檢查是否登入中
    public static bool IsLoggedIn()
    {
        return auth != null && auth.CurrentUser != null;
    }
}
