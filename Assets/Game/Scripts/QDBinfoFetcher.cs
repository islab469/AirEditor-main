using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class QDBinfoFetcher : MonoBehaviour
{
    
    string url = "http://127.0.0.1:8000/unitydata/QDBinf/";
    void Start()
    {
        
        
        StartCoroutine(GetQDBinfo());
        
        print("QDBinfoFetcher work");
    }
    private IEnumerator GetQDBinfo()
    {
        while (FirebaseManager.getEmail() == null)
        {
            print("Waiting for Firebase...");
            yield return new WaitForSeconds(0.5f);
        }
        string uid = FirebaseManager.getEmail();
        string jsonBody = JsonConvert.SerializeObject(new { uid = uid });
        print($"Sending JSON: {jsonBody}");
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                DjangoResponse response = JsonConvert.DeserializeObject<DjangoResponse>(jsonResponse);

                // 清空靜態變數並存入新資料
                QDBManager.FileList.Clear();
                foreach (var file in response.files)
                {
                    QDBManager.FileList.Add(new QDBManager.FileData
                    {
                        filename = file.filename,
                        modified_time = file.modified_time
                    });
                }

                Debug.Log("Files received and stored.");
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
