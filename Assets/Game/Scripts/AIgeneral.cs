using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;

public class AIgeneral : MonoBehaviour
{
    public TMP_InputField inputField; // �s���� UI �� Input Field
    public TMP_InputField topicField;
    
    public void getUserContent()
    {
        
        string content = inputField.text; // ����Τ��J
        string topic = topicField.text;
        int qtype = PlayerPrefs.GetInt("Qtype", 0);
        string filename = PlayerPrefs.GetString("selected_file", "");
        // �ˬd�O�_���ũ� null
        if (string.IsNullOrEmpty(content))
        {
            Debug.Log("��J���e���šA�п�J���e");
            return; // �p�G���šA�h�X���
        }
        
        // �ϥ� StartCoroutine �ӱҰʨ�{
        StartCoroutine(UploadContent(content,qtype,topic,filename));

        Debug.Log("getUserContent Called");
        
    }

    // ��{�G�W�Ǥ��e�ܫ��w�� URL
    IEnumerator UploadContent(string content,int qtype,string topic, string filename)
    {
        string url = "http://127.0.0.1:8000/unitydata/upload_content/"; // ��אּ�A���W�� API
        WWWForm form = new WWWForm(); // �Ыطs�����
        
        string id = FirebaseManager.getEmail(); // �����e�Τ᪺ email

        // �ˬd id �O�_�� null
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("�Τ� ID ���šA�L�k�W�Ǥ��e�C");
            yield break; // �p�G id ���šA�h�X��{
        }

        form.AddField("userid", id); // �K�[�Τ� ID ����
        form.AddField("qtype", qtype);
        form.AddField("topic",topic);
        form.AddField("filename", filename);
        form.AddField("content", content); // �K�[���e����
        Debug.Log($"�W�Ǫ����e: {content}"); // ��x��X�W�Ǥ��e

        using (UnityWebRequest www = UnityWebRequest.Post(url, form)) // �ϥ� POST �ШD�W��
        {
            // www.timeout = 10; // �i��G�]�w 10 ��W��
            yield return www.SendWebRequest(); // �o�e�ШD�õ��ݦ^��

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                DjangoResponse response = JsonConvert.DeserializeObject<DjangoResponse>(jsonResponse);

                // �M���R�A�ܼƨæs�J�s���
                QDBManager.FileList.Clear();
                foreach (var file in response.files)
                {
                    QDBManager.FileList.Add(new QDBManager.FileData
                    {
                        filename = file.filename,
                        modified_time = file.modified_time
                    });
                }
            }
            else
            {
                // �W�Ǧ��\�A��X�^���H��
                Debug.Log($"�W�ǧ����I�^��: {www.downloadHandler.text}");
                SceneSystem.changeScene(SceneType.SCENE_AIQUESTION);
            }
        }
    }
    public class DjangoResponse
    {
        public List<QDBManager.FileData> files;
    }
}
