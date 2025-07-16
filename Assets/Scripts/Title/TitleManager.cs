using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro用

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startText; // InspectorでTextMeshProをアタッチ
    private float blinkTimer = 0f;
    private float blinkInterval = 0.8f; // 点滅間隔（秒）

    void Update()
    {
        // Zキーでシーン遷移
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SceneManager.LoadScene("Scene1");
        }

        // 点滅処理
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            startText.enabled = !startText.enabled; // 表示/非表示を切り替え
            blinkTimer = 0f;
        }
    }
}