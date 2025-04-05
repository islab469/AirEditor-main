using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // �[�J�����޲z���R�W�Ŷ�

public class Spinner : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;

    // Firestore �Ѧ�
    private FirebaseFirestore firestore;

    private void Start()
    {
        // ��l�� Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                firestore = FirebaseFirestore.GetInstance(app);
                Debug.Log("Firebase ��l�Ʀ��\");
                StartCoroutine(GetOptionsFromFirestore());
            }
            else
            {
                Debug.LogError($"Firebase ��l�ƥ���: {task.Result}");
            }
        });

        // ��ť�U�Կ���ܤ�
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private IEnumerator GetOptionsFromFirestore()
    {
        // Firestore ���|
        string collectionPath = "uploads"; // Firestore ���s�x�ﶵ�����X�W

        // �}�l�q Firestore ��������
        CollectionReference optionsRef = firestore.Collection(collectionPath);
        var getOptionsTask = optionsRef.GetSnapshotAsync();

        // ���� Firestore ����������
        yield return new WaitUntil(() => getOptionsTask.IsCompleted);

        if (getOptionsTask.Exception != null)
        {
            Debug.LogError("�q Firestore �����ƮɥX�{���~: " + getOptionsTask.Exception);
        }
        else
        {
            // �q Firestore �������
            QuerySnapshot snapshot = getOptionsTask.Result;

            // �N Firestore ��������ഫ�� List<string>
            List<string> options = new List<string>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    // ���]�C�Ӥ��ɤ����@�� "image_url" �r�q
                    string option = document.GetValue<string>("image_url");
                    options.Add(option);
                }
            }

            // ��s�U�Կ��ﶵ
            UpdateDropdownOptions(options);
        }
    }

    private void UpdateDropdownOptions(List<string> options)
    {
        // �M�ŷ�e�ﶵ
        dropdown.options.Clear();

        // �K�[�ʺA�ﶵ
        foreach (string option in options)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(option));
        }

        // �]�m�q�{�ﶵ
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    private void OnDropdownValueChanged(int index)
    {
        // �ھڿ襤���ﶵ�i��ާ@
        string selectedOption = dropdown.options[index].text;
        Debug.Log($"�襤���ﶵ: {selectedOption}");

        // �����������������W��
        SceneManager.LoadScene(7);
    }
}
