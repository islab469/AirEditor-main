using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

using static QuestionManager;

public class RequestManager
{
    // 🖥️ 改成伺服器真實 IP（不要用 127.0.0.1）
    private static string serverIP = "http://120.101.10.105:8000";

    private static string genQuestionUrl = serverIP + "/unitydata/save_question_by_content/";
    private static string fetchQuestionUrl = serverIP + "/unitydata/fetch_question_by_email/";
    private static string csrfUrl = serverIP + "/unitydata/get_csrf_token/";

    // ✅ 封裝所有 POST 請求 + CSRF 處理
    public static async Task<string> sendRequest(string url, Dictionary<string, string> contents)
    {
        string jsonData = JsonConvert.SerializeObject(contents);
        Debug.Log("🔄 JSONDATA: " + jsonData);

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");

            string token = await get_csrf_token();
            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("❌ 無法取得 CSRF token，請確認伺服器可連線");
                return null;
            }

            request.SetRequestHeader("X-CSRFToken", token);

            var asyncOperation = request.SendWebRequest();
            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ 請求失敗：" + request.error);
                return null;
            }
            else
            {
                Debug.Log("✅ 回應：" + request.downloadHandler.text);
                return request.downloadHandler.text;
            }
        }
    }

    // ✅ 寫入 Django 的題目生成請求
    public static async void sendRequestToGeneateQuestion(string email, string content)
    {
        Dictionary<string, string> testDict = new Dictionary<string, string>
        {
            { "content", content },
            { "email", email }
        };

        await sendRequest(genQuestionUrl, testDict);
        Debug.Log("✅ 題目產生請求完成");
    }

    // ✅ 測試題目生成（可刪除或保留作為測試）
    public static async void sendTestRequestToGeneateQuestion()
    {
        string testContent = "這是一段測試內容，用於題目產生";
        string testEmail = "asd123@gmail.com";

        await Task.Run(() =>
        {
            sendRequestToGeneateQuestion(testEmail, testContent);
        });
    }

    // ✅ 抓取題目資料（由 email 對應）
    public static async Task<Questions> sendRequestToFetchQuestion(string email)
    {
        Dictionary<string, string> testDict = new Dictionary<string, string>
        {
            { "email", email }
        };

        string jsonResponse = await sendRequest(fetchQuestionUrl, testDict);
        if (string.IsNullOrEmpty(jsonResponse))
        {
            Debug.LogWarning("⚠️ 回傳為空");
            return null;
        }

        Debug.Log("📥 jsonResponse: " + jsonResponse);
        Questions qs = JsonUtility.FromJson<Questions>(jsonResponse);
        Debug.Log("✅ 題目下載完成");
        QuestionManager.printQuestions(qs);
        return qs;
    }

    // ✅ 測試抓題（可供 debug 用）
    public static async Task<Questions> sendTestRequestToFetchQuestion()
    {
        string testEmail = "asd123@gmail.com";
        return await sendRequestToFetchQuestion(testEmail);
    }

    // ✅ 向 Django 取得 CSRF token
    private static async Task<string> get_csrf_token()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(csrfUrl))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ Error getting CSRF token: {request.error}");
                return null;
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("🔐 CSRF jsonResponse: " + jsonResponse);

                CsrfTokenResponse response = JsonUtility.FromJson<CsrfTokenResponse>(jsonResponse);
                return response.csrfToken;
            }
        }
    }

    [System.Serializable]
    private class CsrfTokenResponse
    {
        public string csrfToken;
    }
}
