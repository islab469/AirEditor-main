using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Firestore;
using System.Threading.Tasks;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

public class AnswerClick : MonoBehaviour
{
    private FirebaseFirestore db;
    private DocumentReference docRef;

    // �奻��
    public TextMeshProUGUI[] textObj_arr = new TextMeshProUGUI[12];

    // ����D
    public TextMeshProUGUI[] textAns_arr = new TextMeshProUGUI[4];

    // ��e�n��R�� textAns_arr ����
    private int currentAnsIndex = 0; // �_�l�Ȭ� 0

    // �Ψ���ܿ�ܵ��ת��奻
    public TextMeshProUGUI QuestionAndAnswer;
    public Dictionary<string, string> data = new Dictionary<string, string>();
    [SerializeField]
    private bool is_debugging = true;



    private List<object> createTestList(string frontval,int maxval) {

        List<object> strs = new List<object>();
        for (int i = 0; i < maxval; i++) {
            strs.Add(frontval + " "  + maxval.ToString());
        }
        return strs;
    }
    private async void Start()
    {
        const int q_count = 6;
        Dictionary<string, object> doc = new Dictionary<string, object>{
            { "truthes",createTestList("TRUTH", q_count)},
            { "falses",createTestList("FALSE", q_count)}
        };
        if (!is_debugging)
        {
            //MainScene.check_and_instantiate();
            string user = FirebaseManager.getEmail();
            // ��l�� Firebase
            db = FirebaseFirestore.DefaultInstance;
            //docRef = db.Collection("AIContent").Document(User);
            doc = await GetUserDocument(user);  // �եΨ�ƨӨ��^�ƾ�
        }

        //Load the object from firebase 
        List<object> truthesList = doc["truthes"] as List<object>;
        List<object> falsesList = doc["falses"] as List<object>;

        if (truthesList != null)
        {
            // �N�C�Ӥ����ഫ���r��æX��
            string truthesStr = String.Join("�C", truthesList.Select(item => item.ToString()));
            truthesStr = truthesStr.Replace("\n", "").Replace("\n", "");
            string[] truthes = truthesStr.Split("�C");
            print(truthes.Length);
            print(textObj_arr.Length);
            print(gameObject.name);

            for (int i = 0; i < 6; i++)
            {
                //#ERR
                textObj_arr[i].text = truthes[i];
            }
        }
        else
        {
            Debug.Log("truthes ���O List<object> ����");
        }

        if (falsesList != null)
        {
            // �N�C�Ӥ����ഫ���r��æX��

            string falsesStr = String.Join("�C", falsesList.Select(item => item.ToString()));
            falsesStr = falsesStr.Replace("\n", "").Replace("\r", "");
            string[] falses = falsesStr.Split("�C");
            for (int i = 0; i < 6; i++)
            {
                textObj_arr[i+6].text = falses[i];
            }
            // ��ܦX�᪺֫�r��
        }
        else
        {
            Debug.Log("truthes ���O List<object> ����");
        }



        //for (int i = 0; i < textObj_arr.Length; i++)
        //{
        //    textObj_arr[i].text = 
        //    Debug.Log(strs[i]);
        //}

    }

    private async Task<Dictionary<string, object>> GetUserDocument(string userId)
    {
        
        // ���V Firestore �� AIContent ���X���� userId ����
        DocumentReference docRef = db.Collection("AIContent").Document(userId);
     
        Dictionary<string, object> documentData = new Dictionary<string, object>();


        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            // ���X���ɪ��ƾڡA�ñN���ഫ���r��
            documentData = snapshot.ToDictionary();

        }


        return documentData;
    }

    // �ΨӦb���s�I����Ū���奻����ܪ����
    public void OnAnswerClick(int answerIndex)
    {
        // �ˬd���ެO�_�b�d��
        if (answerIndex >= 0 && answerIndex < textObj_arr.Length)
        {
            // ����襤���奻
            string selectedText = textObj_arr[answerIndex].text;

            // �ˬd textAns_arr �O�_�٦�����J���奻
            if (currentAnsIndex < textAns_arr.Length)
            {
                // �N�襤���奻�ƻs�� textAns_arr ����e����
                textAns_arr[currentAnsIndex].text = selectedText;

                // ��s���ޡA�ǳƤU�@���I��
                currentAnsIndex++;
            }
            else
            {
                Debug.LogWarning("All answer slots are filled.");
            }
        }
        else
        {
            Debug.LogError("Index is out of bounds: " + answerIndex);
        }
    }
}
