using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class QDBinfoFetcher : MonoBehaviour
{
    private ClientWebSocket ws;
    private string userId;
    private CancellationTokenSource cts = new CancellationTokenSource();

    // Django 後端 API 端點
    private string apiUrl = "http://127.0.0.1:8000/unitydata/QDBinf/";

    void Start()
    {
        StartCoroutine(FetchQDBInfo());
        Debug.Log("QDBinfoFetcher started.");
    }

    /// <summary>
    /// 發送 POST 請求到 Django API，取得目前使用者的檔案清單
    /// </summary>
    private IEnumerator FetchQDBInfo()
    {
        // 等待 Firebase 登入成功，獲取 email
        while (string.IsNullOrEmpty(FirebaseManager.GetEmail()))
        {
            yield return new WaitForSeconds(0.5f);
        }

        userId = FirebaseManager.GetEmail();
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is null or empty.");
            yield break;
        }

        // 建立 JSON 請求內容
        string jsonBody = JsonConvert.SerializeObject(new { uid = userId });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        // 設定 UnityWebRequest 並附加 Header
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 發送請求
            yield return request.SendWebRequest();

            // 處理回應
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonResponse = request.downloadHandler.text;

                    if (string.IsNullOrEmpty(jsonResponse))
                    {
                        Debug.LogWarning("Django API returned empty response.");
                        yield break;
                    }

                    DjangoResponse response = JsonConvert.DeserializeObject<DjangoResponse>(jsonResponse);

                    if (response?.files != null)
                    {
                        QDBManager.FileList.Clear();
                        foreach (var file in response.files)
                        {
                            QDBManager.FileList.Add(new QDBManager.FileData
                            {
                                filename = file.filename,
                                modified_time = file.modified_time
                            });
                        }

                        Debug.Log("QDB file list successfully fetched and updated.");
                    }
                    else
                    {
                        Debug.LogWarning("Response file list is null or empty.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to parse JSON response: " + e.Message);
                }
            }
            else
            {
                Debug.LogError($"Error fetching files from Django API: {request.error}");
            }
        }
    }

    /// <summary>
    /// Django 後端傳回的資料格式
    /// </summary>
    public class DjangoResponse
    {
        public List<QDBManager.FileData> files;
    }
}
