using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEditor.Build.Content;
using Firebase.Extensions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.CompilerServices; // 引入 Firestore 命名空間

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
    public static FirebaseAuth auth; // Firebase 認證實例
    public static FirebaseFirestore firestore; // Firestore 參考
    public static FirebaseUser user; // 當前用戶
    public static DatabaseReference databaseReference; // 實時數據庫引用


    public static string email;
    public static string password;
    public  GameObject PanelLogin;
    public  GameObject PanelSelection;

    void Start()
    {
       //PanelSelection.SetActive(false);
    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void checkAndStart()
    {
        if (auth != null)
        {

            return;
        }
        // 獲取 Firebase 認證的默認實例
        auth = FirebaseAuth.DefaultInstance;

        // 確保 Firebase Auth 已初始化
        if (auth == null)
        {
            Debug.LogError("FirebaseAuth 物件為 null");
            return;
        }

        // 訂閱認證狀態變化的事件
        //Handle the login 
        auth.StateChanged += AuthStateChanged;

        // 初始化 Firestore
        firestore = FirebaseFirestore.DefaultInstance;
        if (firestore == null)
        {
            Debug.LogError("Firestore 初始化失敗。");
            return;
        }

        Debug.Log("Firebase 初始化成功");
    }

    // 註冊新用戶
    public static async Task<bool> Register(string email, string password)
    {
        // 創建用戶
        AuthResult authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);

        WriteUserToFirestore(email, "New User");
        Debug.Log("用戶資料寫入 Firestore 成功！");
        return true;
    }

    // 用戶登錄
    public async static Task<bool> Login(string email, string password)
    {
        // 登錄用戶
        AuthResult ar = await auth.SignInWithEmailAndPasswordAsync(email, password);
        return isLogin();
    }

    // 用戶登出
    public static void Logout()
    {
        auth.SignOut();
    }

    // 認證狀態變化處理
    private static void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        // 檢查當前用戶是否發生變化
        if (auth.CurrentUser != user)
        {
            user = auth.CurrentUser;
            if (user != null)
            {
                Instance.PanelLogin.SetActive(false);
                Instance.PanelSelection.SetActive(true);
                Debug.Log($"用戶已登錄 - {user.Email}");
            }
            else
            {
                Instance.PanelLogin.SetActive(true);
                Instance.PanelSelection.SetActive(false);
                Debug.Log("用戶已登出");
            }
        }
    }
    // 寫入新用戶資料到 Firestore
    public static async void WriteUserToFirestore(string email, string displayName)
    {

        if (firestore == null)
        {
            Debug.LogError("Firestore 物件為 null，無法寫入數據。");
            return;
        }

        User newUser = new User(email, displayName);
        Debug.Log("準備寫入用戶資料至 Firestore");

        DocumentReference document = firestore.Collection("users").Document(email);


        Debug.Log("獲取 DocumentReference 成功");

        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        keyValuePairs.Add("email", email);

        await document.SetAsync(newUser.getDicionary()); // 寫入用戶資料
    }

    // User 類別用於資料結構

    // get email with error checking
    public static string getEmail()
    {

        return user != null ? user.Email : null;
    }

    public static bool isLogin()
    {

        return auth != null && auth.CurrentUser != null;
    }

}