using System;
using System.Text;
using UnityEngine;
using NativeWebSocket;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket websocket;

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
            }
            catch (Exception ex)
            {
                Debug.LogError("❌ JSON 解析失敗: " + ex.Message);
            }
        };

        await websocket.Connect();
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
