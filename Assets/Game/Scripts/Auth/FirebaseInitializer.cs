using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseInitializer : MonoBehaviour
{
    public static bool IsFirebaseReady { get; private set; } = false;

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                Debug.Log("Firebase initialized successfully.");
                IsFirebaseReady = true;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                IsFirebaseReady = false;
            }
        });
    }
}
