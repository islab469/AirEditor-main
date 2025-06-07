using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoginTransition : MonoBehaviour
{
    public CanvasGroup fadeGroup;      // 放 UI 總容器（含背景）
    public TextMeshProUGUI statusText; // 放登入中的文字
    public float fadeDuration = 1.5f;  // 淡入持續秒數
    public string nextSceneName = "Lobby"; // 下個場景名

    void Start()
    {
        // 先把 UI 全部透明
        fadeGroup.alpha = 0;
        StartCoroutine(PlayLoginAnimation());
    }

    IEnumerator PlayLoginAnimation()
    {
        // 淡入畫面
        float time = 0;
        while (time < fadeDuration)
        {
            fadeGroup.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        fadeGroup.alpha = 1;

        // 顯示狀態文字
        statusText.text = "登入中...";
        yield return new WaitForSeconds(1.2f); // 可自行調整停留秒數

        // 切換場景
        SceneManager.LoadScene(nextSceneName);
    }
}
