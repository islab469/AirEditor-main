using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class AIgeneral : MonoBehaviour
{
    public TMP_InputField inputField; // �s���� UI �� Input Field

    public TMP_Dropdown[] dropdowns = new TMP_Dropdown[3];
    // �o�Ӥ�k�N�Q���s�� OnClick �ƥ�ե�
    public void getUserContent()
    {
        
        string content = inputField.text; // ����Τ��J
        
        // �ˬd�O�_���ũ� null
        if (string.IsNullOrEmpty(content))
        {
            Debug.Log("��J���e���šA�п�J���e");
            return; // �p�G���šA�h�X���
        }
        print(dropdowns[0].value);
        // �ϥ� StartCoroutine �ӱҰʨ�{
        StartCoroutine(UploadContent(content, dropdowns[0].value, dropdowns[1].value, dropdowns[2].value));

        Debug.Log("getUserContent Called");
        
    }

    // ��{�G�W�Ǥ��e�ܫ��w�� URL
    IEnumerator UploadContent(string content,int tf_count, int choose_count, int assay_count)
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
        form.AddField("content", content); // �K�[���e����
        form.AddField("tf_count", tf_count);
        form.AddField("choose_count", choose_count);
        form.AddField("assay_count", assay_count);
        Debug.Log($"�W�Ǫ����e: {content}"); // ��x��X�W�Ǥ��e

        using (UnityWebRequest www = UnityWebRequest.Post(url, form)) // �ϥ� POST �ШD�W��
        {
            // www.timeout = 10; // �i��G�]�w 10 ��W��
            yield return www.SendWebRequest(); // �o�e�ШD�õ��ݦ^��

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                // �ШD���~�A��X���~�H��
                Debug.LogError($"�W�ǿ��~: {www.error}\n�^��: {www.downloadHandler.text}");
            }
            else
            {
                // �W�Ǧ��\�A��X�^���H��
                Debug.Log($"�W�ǧ����I�^��: {www.downloadHandler.text}");
                SceneSystem.changeScene(SceneType.SCENE_AIQUESTION);
            }
        }
    }
}
