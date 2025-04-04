using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public GameObject popupPanel; // 指定 Panel
    public Button button1;
    public Button button2;
    public Button button3;
    public Button closeButton; // 新增一個關閉按鈕

    void Start()
    {
        // 確保面板一開始是隱藏的
        popupPanel.SetActive(false);

        // 綁定按鈕事件
        closeButton.onClick.AddListener(ClosePopup);
        button1.onClick.AddListener(SwitchToGenTF);
        button2.onClick.AddListener(SwitchToGenChoose);
        button3.onClick.AddListener(SwitchToGenAssay);
    }

    public void OpenPopup()
    {
        popupPanel.SetActive(true);
    }
    void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
    void SwitchToGenTF()
    {
        PlayerPrefs.SetInt("Qtype", 0);
        SceneManager.LoadScene(2);
    }

    void SwitchToGenChoose()
    {
        PlayerPrefs.SetInt("Qtype", 2);
        SceneManager.LoadScene(2);
    }

    void SwitchToGenAssay()
    {
        PlayerPrefs.SetInt("Qtype", 4);
        SceneManager.LoadScene(2);
    }

}
