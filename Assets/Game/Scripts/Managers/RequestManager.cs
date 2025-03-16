using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

using static QuestionManager;

public class RequestManager{
    private static string genQuestionUrl = "http://127.0.0.1:8000/unitydata/save_question_by_content/";
    private static string fetchQuestionUrl = "http://127.0.0.1:8000/unitydata/save_question_by_content/";

    public static async Task<string> sendRequest(string url, Dictionary<string, string> contents) {
        string jsonData = JsonConvert.SerializeObject(contents);
        //when it finished , it will release the resource
        Debug.Log("JSONDATA:" + jsonData);
        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData)) { 
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            string token = await get_csrf_token();
            Debug.Log("CSRF - TOKEN:" + token);
            request.SetRequestHeader("X-CSRFToken", token);

            AsyncOperation asyncOperation = request.SendWebRequest();
            while (!asyncOperation.isDone){
                await Task.Yield();//wait for next frame :)
            }


            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("something wrong in RequestManaer sendRequest!");
                return null;
            }
            else {
                Debug.Log(request.downloadHandler.text);
                return request.downloadHandler.text;
            }
        }

    }



    public static async void sendRequestToGeneateQuestion(string email,string content) {
        Dictionary<string, string> testDict = new Dictionary<string, string>();
        testDict.Add("content", content);
        testDict.Add("email", email);
        await sendRequest(genQuestionUrl, testDict);
        Debug.Log("Question generation done!");
    }

    public static async void sendTestRequestToGeneateQuestion() {
        string testContent = "艾薩克·牛頓爵士 PRS MP（英語：Sir Isaac Newton，發音：/ˈaɪzək ˈnjuːtn̩/，儒略曆：1642年12月25日—1727年3月20日[a]；格里曆：1643年1月4日—1727年3月31日）是英國物理學家、數學家、天文學家、自然哲學家及輝格黨政治人物。1687年他發表《自然哲學的數學原理》，闡述了萬有引力和三大運動定律，由此奠定現代物理學和天文學，並為現代工程學打下了基礎。他通過論證克卜勒行星運動定律與他的重力理論間的一致性，展示了地面物體與天體的運動都遵循著相同的自然定律；為太陽中心學說提供了強而有力的理論支持，是科學革命的一大代表。\r\n\r\n在力學上，牛頓闡明了動量和角動量守恆的原理。在光學上，他發明了反射望遠鏡，並基於對三稜鏡將白光發散成可見光譜的觀察，發展出了顏色理論。他還系統地表述了冷卻定律，並研究了音速。\r\n\r\n在數學上，牛頓與哥特弗利德·萊布尼茲分享了發展出微積分學的榮譽。他也證明了廣義二項式定理，提出了「牛頓法」以趨近函數的零點，並為冪級數的研究作出了貢獻。\r\n\r\n在2005年，英國皇家學會發起了一場「誰是科學史上最有影響力的人」的民意調查，在皇家學會院士和網民投票中，牛頓獲得頭冠。[3]";
        string testEmail = "asd123@gmail.com";

        await Task.Run(() => { sendRequestToGeneateQuestion(testEmail, testContent); });
    }


    public static async Task<Questions> sendRequestToFetchQuestion(string email) {
        Dictionary<string, string> testDict = new Dictionary<string, string>();
        testDict.Add("email", email);
        string jsonResponse = await sendRequest(fetchQuestionUrl, testDict);
        //response array of questions
        Debug.Log("jsonResponse : " + jsonResponse.ToString());
        //"data"->arr->titles...
        Questions qs = JsonUtility.FromJson<Questions>(jsonResponse);
        Debug.Log("Question Fetch done!");
        QuestionManager.printQuestions(qs);
        return qs;
    }

    public static async Task<Questions> sendTestRequestToFetchQuestion()
    {
        string testEmail = "asd123@gmail.com";
        return await sendRequestToFetchQuestion(testEmail);
        
    }


    private static async Task<string> get_csrf_token()
    {
        string csrfUrl = "http://127.0.0.1:8000/unitydata/get_csrf_token/";
        using (UnityWebRequest request = UnityWebRequest.Get(csrfUrl))
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();  // 等待請求完成
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error getting CSRF token: {request.error}");
                return null;
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("CSRF jsonResponse : " + jsonResponse);

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
