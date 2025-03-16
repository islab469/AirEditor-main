using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestionLoader : MonoBehaviour
{
    public GameObject QSprefab;
    public Transform content;

    void Start()
    {
        
    }

    void creatPrefab()
    {
        if (QDBManager.FileList.Count > 0)
        {
            foreach (var file in QDBManager.FileList)
            {
                GameObject newPrefab = Instantiate(QSprefab, content);
                Button button = newPrefab.GetComponentInChildren<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnPrefabClicked());
                }
            }
        }
        else
        {
            print("No files in QDBManager.FileList");
        }
    }

    void OnPrefabClicked()
    {
        SceneManager.LoadScene(1);
    }
}

