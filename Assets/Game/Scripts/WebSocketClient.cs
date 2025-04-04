using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;


public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;

    async void Start()
    {
        ws = new WebSocket("ws://your-django-server/ws/questions/");

        ws.OnMessage += (message) =>
        {
            string jsonMessage = Encoding.UTF8.GetString(message);
            Debug.Log("Received Question Update: " + jsonMessage);

            try
            {
                List<QDBManager.FileData> receivedFiles = JsonConvert.DeserializeObject<List<QDBManager.FileData>>(jsonMessage);
                QDBManager.FileList.Clear();
                QDBManager.FileList.AddRange(receivedFiles);

                Debug.Log("Updated QDBManager.FileList with " + receivedFiles.Count + " files.");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse JSON: " + e.Message);
            }
        };

        await ws.Connect();
    }

    private async void OnApplicationQuit()
    {
        await ws.Close();
    }
}
