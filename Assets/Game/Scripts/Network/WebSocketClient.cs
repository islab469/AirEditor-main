using System;
using System.Text;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.UI;  // 引入 UI 元素類別
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket websocket;
    public GameObject popupPanel;  // 用來顯示彈出視窗的 Panel
    public TextMeshProUGUI notificationText;  // 用來顯示通知訊息的 Text 元素

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;  // 訂閱場景加載事件
    }

    // 當物件禁用時，取消訂閱場景加載事件
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  // 取消訂閱場景加載事件
    }

    // 當新場景加載完成後，重新設置 UI 元素
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 確保在場景加載後，popupPanel 物件初始隱藏
        popupPanel.SetActive(false); // 初始化為隱藏
        notificationText.text = "";  // 初始化為空，顯示空的文字
    }

    async void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Application.runInBackground = true;

        string serverUrl = "ws://120.101.10.105:8000/ws/questions/"; // 改成你的 Django WebSocket URL
        websocket = new WebSocket(serverUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("✅ WebSocket 已連線！");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("❌ WebSocket 錯誤: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.LogWarning("⚠️ WebSocket 已關閉，代碼：" + e);
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            Debug.Log("📩 收到訊息: " + message);

            try
            {
                QDBManager.FileData data = JsonUtility.FromJson<QDBManager.FileData>(message);
                QDBManager.FileList.Add(data);
                Debug.Log("✅ 已加入 QDBManager: " + data.filename);

                // 顯示題目生成完畢的訊息
                ShowPopup();
            }
            catch (Exception ex)
            {
                Debug.LogError("❌ JSON 解析失敗: " + ex.Message);
            }
        };

        await websocket.Connect();
    }

    private void ShowPopup()
    {
        // 顯示彈出視窗並修改 text 內容
        if (notificationText != null)
        {
            popupPanel.SetActive(true);  // 顯示 panel
            notificationText.text = "題目生成完畢";  // 顯示訊息

            // 等待 3 秒後清空文字並隱藏 panel
            StartCoroutine(ClearTextAfterDelay(3f));  // 3秒後清空文字
        }
    }

    private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (notificationText != null)
        {
            notificationText.text = "";  // 清空文字
            popupPanel.SetActive(false);  // 隱藏 panel
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
