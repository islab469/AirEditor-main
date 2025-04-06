using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using System.Threading.Tasks;
using Firebase.Extensions;
using FirestoreModels;
using Firebase;
using System;
using System.Text.RegularExpressions;

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

    // 初始化 Firebase
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

    public static async Task<string> Login(string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return "Email or password cannot be empty.";

            AuthResult ar = await auth.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log("Login successful: " + ar.User.Email);
            return "success";
        }
        catch (FirebaseException fe)
        {
            return "Login failed: " + fe.Message;
        }
        catch (Exception ex)
        {
            return "Login failed: " + ex.Message;
        }
    }

    public static async Task<string> Register(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return "Email or password cannot be empty.";

        if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$"))
            return "Password must include uppercase, lowercase, digit, and special character (min 8 chars).";

        try
        {
            AuthResult authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            await WriteUserToFirestore(email, "New User");
            return "success";
        }
        catch (FirebaseException fe)
        {
            if (fe.Message.Contains("email-already-in-use"))
                return "email-already-in-use";
            return "Registration failed: " + fe.Message;
        }
        catch (Exception ex)
        {
            return "Registration failed: " + ex.Message;
        }
    }



    // 登出
    public static void Logout()
    {
        auth.SignOut();
    }

    // Firebase 狀態改變事件（登入/登出 UI 切換）
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

    // 將用戶資料寫入 Firestore
    public static async Task WriteUserToFirestore(string email, string displayName)
    {
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

    // 回傳登入的 Email
    public static string GetEmail() => user?.Email;

    // 是否已登入
    public static bool IsLoggedIn() => auth != null && auth.CurrentUser != null;

    // 驗證密碼是否含有特殊符號
    private static bool HasSpecialChar(string input) => System.Text.RegularExpressions.Regex.IsMatch(input, @"[!@#$%^&*(),.?""{}|<>]");

    // 錯誤解析
    private static string ParseFirebaseError(FirebaseException fe)
    {
        var errorCode = ((AuthError)fe.ErrorCode).ToString();
        switch (errorCode)
        {
            case "InvalidEmail": return "Invalid email format.";
            case "WrongPassword": return "Incorrect password.";
            case "EmailAlreadyInUse": return "Email is already registered.";
            case "UserNotFound": return "User not found.";
            case "WeakPassword": return "Password is too weak.";
            default: return $"Firebase error: {errorCode}";
        }
    }
}
