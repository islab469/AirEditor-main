using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestionLoader : MonoBehaviour
{
    public GameObject filePrefab;  // 試卷的 Prefab
    public Transform content;      // 存放 Prefab 的父物件
    public Button addButton;       // 新增按鈕

    private string localPath; // 本地端儲存路徑

    void Start()
    {
        localPath = Application.persistentDataPath + "/QDBFiles/";

        // 確保資料夾存在
        if (!Directory.Exists(localPath))
        {
            Directory.CreateDirectory(localPath);
        }
        GeneratePrefabs();
        // 按下「新增按鈕」時，執行 `AddNewFile()`
        if (addButton != null)
        {
            addButton.onClick.AddListener(AddNewFile);
        }

        
    }

    void AddNewFile()
    {
        string filename = "NewFile_" + (QDBManager.FileList.Count + 1) + ".json";
        string filePath = localPath + filename;

       
        File.WriteAllText(filePath, "");

        
        QDBManager.FileList.Add(new QDBManager.FileData
        {
            filename = filename,
            modified_time = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        });

        CreateFilePrefab(QDBManager.FileList[QDBManager.FileList.Count - 1]);
    }


    void CreateFilePrefab(QDBManager.FileData file)
    {
        GameObject newPrefab = Instantiate(filePrefab, content);

        // 設定 Prefab 內的 `TextMeshProUGUI`
        TextMeshProUGUI filenameText = newPrefab.GetComponentInChildren<TextMeshProUGUI>();
        if (filenameText != null)
        {
            filenameText.text = file.filename;
        }

        Button button = newPrefab.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnFileClicked(file.filename));
        }
    }

    void OnFileClicked(string filename)
    {
        Debug.Log($"Opening file: {filename}");
        PlayerPrefs.SetString("selected_file", filename);  // 記住選擇的檔案
        SceneManager.LoadScene(5);
    }
    void GeneratePrefabs()
    {
        // 先清空舊的 Prefab，防止重複生成
        foreach (Transform child in content)
        {
            if (child.gameObject != addButton.gameObject) // 不刪除新增按鈕
            {
                Destroy(child.gameObject);
            }
        }

        if (QDBManager.FileList.Count > 0)
        {
            foreach (var file in QDBManager.FileList)
            {
                CreateFilePrefab(file);
            }
        }
        else
        {
            Debug.Log("No files found in QDBManager.FileList.");
        }
    }

}

