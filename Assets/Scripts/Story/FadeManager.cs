using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private GameObject fadePanel;
    private static FadeManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (fadePanel != null)
        {
            fadePanel.SetActive(false);
        }

        // メインシーン開始時に暗転をトリガー
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainScene")
        {
            StartCoroutine(ShowFadeOnMainScene());
        }
    }

    private IEnumerator ShowFadeOnMainScene()
    {
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            Image fadeImage = fadePanel.GetComponent<Image>();
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, 1f); // 即座に黒
                Debug.Log("FadeManager - Fade started on MainScene");
                yield return new WaitForSeconds(5f); // 5秒待機
                fadeImage.color = new Color(0, 0, 0, 0f); // 透明に戻す
                fadePanel.SetActive(false);
                Debug.Log("FadeManager - Fade ended on MainScene");
            }
        }
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            Image fadeImage = fadePanel.GetComponent<Image>();
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, 0); // 初期透明
                // フェードイン
                for (float alpha = 0f; alpha <= 1f; alpha += Time.deltaTime * 0.5f)
                {
                    fadeImage.color = new Color(0, 0, 0, alpha);
                    yield return null;
                }
                fadeImage.color = new Color(0, 0, 0, 1f);
                Debug.Log("FadeManager - Fade in complete");
                yield return new WaitForSeconds(0.5f);
            }
        }

        SceneManager.LoadScene(sceneName);

        if (fadePanel != null)
        {
            Image fadeImage = fadePanel.GetComponent<Image>();
            if (fadeImage != null)
            {
                // フェードアウト
                for (float alpha = 1f; alpha >= 0f; alpha -= Time.deltaTime * 0.5f)
                {
                    fadeImage.color = new Color(0, 0, 0, alpha);
                    yield return null;
                }
                fadeImage.color = new Color(0, 0, 0, 0f);
                fadePanel.SetActive(false);
                Debug.Log("FadeManager - Fade out complete");
            }
        }
    }
}