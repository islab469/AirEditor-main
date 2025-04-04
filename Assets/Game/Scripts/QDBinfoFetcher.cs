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
    string apiUrl = "http://127.0.0.1:8000/unitydata/QDBinf/";

    void Start()
    {
        StartCoroutine(GetQDBinfo());
        Debug.Log("QDBinfoFetcher started");
    }

    private IEnumerator GetQDBinfo()
    {
        while (FirebaseManager.getEmail() == null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        userId = FirebaseManager.getEmail();
        string jsonBody = JsonConvert.SerializeObject(new { uid = userId });

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                DjangoResponse response = JsonConvert.DeserializeObject<DjangoResponse>(jsonResponse);

                QDBManager.FileList.Clear();
                foreach (var file in response.files)
                {
                    QDBManager.FileList.Add(new QDBManager.FileData
                    {
                        filename = file.filename,
                        modified_time = file.modified_time
                    });
                }
                Debug.Log("Initial QDBinfo received.");
            }
            else
            {
                Debug.LogError("Error fetching files: " + request.error);
            }
        }
    }


    public class DjangoResponse
    {
        public List<QDBManager.FileData> files;
    }
}
