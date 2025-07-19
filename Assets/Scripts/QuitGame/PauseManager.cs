using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas; // インスペクターでPauseCanvasを設定

    void Start()
    {
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false); // 初期非アクティブ
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainScene" && Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseCanvas != null)
            {
                bool isActive = pauseCanvas.activeSelf;
                pauseCanvas.SetActive(!isActive); // Escでトグル（表示/非表示）
                Debug.Log("Quit confirmation " + (isActive ? "deactivated" : "activated"));
                if (!isActive) // 表示された場合のみ入力待ち
                {
                    StartCoroutine(HandleQuitInput());
                }
            }
        }
    }

    private System.Collections.IEnumerator HandleQuitInput()
    {
        while (pauseCanvas.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return)) // Enterキー
            {
                Debug.Log("Quitting Game");
                Application.Quit(); // ゲーム終了
                yield break;
            }
            yield return null;
        }
    }
}