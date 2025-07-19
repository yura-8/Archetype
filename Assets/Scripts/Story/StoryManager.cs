using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private GameObject characterImageRight; // Existing character image for the right side
    [SerializeField] private GameObject characterImageLeft;  // New character image for the left side
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Sprite protagonistNormal, protagonistSerious, protagonistSurprised, protagonistSmile;
    [SerializeField] private Sprite femaleStudentNormal, femaleStudentSad, femaleStudentAngry, femaleStudentRelieved;
    [SerializeField] private Sprite whiteCoatManNormal, whiteCoatManSmug, whiteCoatManAngry;
    [SerializeField] private Collider2D mainTrigger;
    [SerializeField] private GameObject fadePanel; // インスペクターで設定

    private string[] introLines = {
        "転校してきた蓮は学校に向かっていた。",
        "そこは「機星高等学校」である。",
        "蓮は特段この学校に行きたいわけでもなかったが、サイボーグである自身にも通いやすい制度が整っていて、何より近いという理由でここにいくことを決めた。",
        "いい天気だと呑気な事を考えながら歩いていると何やら騒がしい声が聞こえてくる。",
        "黒鉄 蓮「なんだか校門の方が騒がしいな。行ってみよう。」"
    };

    private string[] gateLines = {
        "声のする方向へ行ってみると似たような制服を着た女子生徒と白衣を着た見知らぬ男が言い争っている。",
        "白羽 椿「ロボットは道具ではありません！今すぐそのロボットを返してください！」",
        "女子生徒は男をにらみ付ける。",
        "???「もう既に壊れかけている量産型ロボットじゃないか。この俺が有効活用してやると言っているのだから、寧ろ感謝の言葉を述べるべきだろう？」",
        "白羽 椿「量産型とか関係ありません！直せるロボットをわざわざ壊す必要なんて無いじゃないですか！」",
        "???「これほどのバッテリーならばヤツの動力に使えそうだ。そこまで云うのならば外装は君にあげても良い。」",
        "そう言うと男はポケットからドライバーを取り出した。",
        "白羽 椿「やめて！」",
        "黒鉄 蓮「そのロボットを壊そうっていうなら、俺が相手になってやる！」",
        "白衣の男「なんだお前は？それをこっちに渡せ。」",
        "黒鉄 蓮「断る。」",
        "白衣の男「面倒だな…」",
        "男はスマホを取り出した。",
        "何やら入力すると何処からともなく複数のロボットが少女と蓮を取り囲む。",
        "黒鉄 蓮「やるしかない！」",
        "蓮はカバンから小さな箱を取り出した.",
        "黒鉄 蓮「くらえ！」",
        "蓮はロボットをあっという間に倒してしまった。",
        "白羽 椿「あの数のロボットを倒すなんて…」",
        "白衣の男「クッハハハハハハハハハハ！」",
        "白衣の男「その力…その武器は……いや、辞めておこう。今回はお前の勝ちだ。」",
        "そう言うと男はあっという間にどこかに消えた。",
        "男が見えなくなると蓮は女子生徒の方を向いてロボットを手渡した。",
        "黒鉄 蓮「コレ、取り返したよ。」",
        "白羽 椿「ありがとうございます…！」",
        "女子生徒は工具箱を取り出し、あっという間にロボットを直してしまった。",
        "白羽 椿「自己紹介がまだでしたね。私は2年B組の白羽椿です。」",
        "白羽 椿「貴方、私たちの部活に入りませんか？」",
        "黒鉄 蓮「部活？」"
    };

    private string[] mainLines = {
        "最初の授業が終わり、言われた通りの教室に行くと、そこに椿はいた。",
        "白羽 椿「ようこそ！これが私の所属する部活、G.E.A.R.です！」",
        "そして部員の2人を紹介された。",
        "元気な同級生、強気な後輩だ。",
        "白羽 椿「この部活は暴走したロボットを止める事が主な業務です。」",
        "白羽 椿「早速ですがこの辺りの暴走したロボットを制圧しに行きましょう！」"
    };

    // シーン遷移時の状態を保持
    private static bool isFromGateStory = false;
    private static bool hasPlayedMainStory = false; // メインストーリー初回実行フラグ

    void Start()
    {
        characterImageRight.SetActive(false);
        if (characterImageLeft != null) characterImageLeft.SetActive(false); // Ensure left image is also off
        dialogBox.SetActive(false);
        UpdateCharacterImage(null, null); // Initialize both to transparent
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainScene" && !isFromGateStory && !hasPlayedMainStory)
        {
            StartCoroutine(PlayMainStory());
            hasPlayedMainStory = true; // 初回実行を記録
        }
        else if (currentScene != "MainScene")
        {
            StartCoroutine(PlayIntroStory());
        }
        
        if (fadePanel != null && !fadePanel.activeSelf)
        {
            DontDestroyOnLoad(fadePanel);
            fadePanel.SetActive(false); // 初期非アクティブ
        }
    }

    System.Collections.IEnumerator PlayIntroStory()
    {
        for (int i = 0; i < introLines.Length; i++)
        {
            dialogBox.SetActive(true);
            dialogText.text = introLines[i];
            UpdateCharacterImage(null, null); // No character images in intro
            yield return new WaitForSeconds(3f);
        }
        dialogBox.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Entered: " + gameObject.name + ", Contact with: " + other.name + ", Tag: " + other.tag + ", Player Tag Check: " + other.CompareTag("Player"));
        if (other.CompareTag("Gate"))
        {
            Debug.Log("Starting PlayGateStory");
            StartCoroutine(PlayGateStory());
        }
        else if (other.CompareTag("MainTrigger") && !hasPlayedMainStory && SceneManager.GetActiveScene().name == "MainScene")
        {
            Debug.Log("Starting PlayMainStory from MainTrigger");
            StartCoroutine(PlayMainStory());
            hasPlayedMainStory = true; // 初回実行を記録
        }
    }

    System.Collections.IEnumerator PlayGateStory()
    {
        characterImageRight.SetActive(true);
        if (characterImageLeft != null) characterImageLeft.SetActive(true); // Activate left image for this story

        for (int i = 0; i < gateLines.Length; i++)
        {
            dialogBox.SetActive(true);
            dialogText.text = gateLines[i];
            // Clear both images before setting the current one
            UpdateCharacterImage(null, null); 

            if (i == 1 || i == 4 || i == 7) // Female student speaks
            {
                UpdateCharacterImage(femaleStudentSad, null); // Female student on left, assuming Protagonist is usually on right
            }
            else if (i == 6) // Protagonist surprised
            {
                UpdateCharacterImage(null, protagonistSurprised); // Protagonist on left
            }
            else if (i == 9 || i == 10 || i == 11) // White coat man speaks
            {
                UpdateCharacterImage(whiteCoatManAngry, protagonistSerious); // White Coat Man on right
            }
            else if (i == 8) // Protagonist speaks (or action is focused on them)
            {
                UpdateCharacterImage(null, protagonistSerious); // Protagonist on left or right depending on the scene
            }
            else if (i == 13) // Protagonist surprised
            {
                UpdateCharacterImage(femaleStudentAngry, protagonistSurprised); // Protagonist on left
            }
            else if (i == 14 || i == 16) // Protagonist surprised
            {
                UpdateCharacterImage(null, protagonistSerious); // Protagonist on left
            }
            else if (i == 18) // Protagonist surprised
            {
                UpdateCharacterImage(femaleStudentAngry, null); // Protagonist on left
            }
            else if (i == 19) // Protagonist surprised
            {
                UpdateCharacterImage(whiteCoatManSmug, protagonistSerious); // Protagonist on left
            }
            else if (i == 20) // Protagonist surprised
            {
                UpdateCharacterImage(whiteCoatManNormal, protagonistSerious); // Protagonist on left
            }
            else if (i == 23 || i == 24) // Protagonist surprised
            {
                UpdateCharacterImage(femaleStudentRelieved, protagonistNormal); // Protagonist on left
            }
            else if (i == 26 || i == 27) // Protagonist surprised
            {
                UpdateCharacterImage(femaleStudentNormal, protagonistNormal); // Protagonist on left
            }
            else if (i == 28) // Protagonist surprised
            {
                UpdateCharacterImage(null, protagonistSmile); // Protagonist on left
            }
            else
            {
                UpdateCharacterImage(null, null); // No one speaking, or narrator
            }
            yield return new WaitForSeconds(3f);
        }
        dialogBox.SetActive(false);
        characterImageRight.SetActive(false);
        if (characterImageLeft != null) characterImageLeft.SetActive(false);
        yield return StartCoroutine(ShowBlackScreen());
        isFromGateStory = true; // ゲートストーリーから遷移したことを記録
        StartCoroutine(FadeToMainStory());
    }

    // 黒い画面を3.5秒表示するコルーチン
    private System.Collections.IEnumerator ShowBlackScreen()
    {
        Debug.Log("ShowBlackScreen - FadePanel assigned: " + (fadePanel != null));
        if (fadePanel != null)
        {
            fadePanel.SetActive(true); // 強制的にアクティブ化
            Image fadeImage = fadePanel.GetComponent<Image>();
            Debug.Log("ShowBlackScreen - FadeImage found: " + (fadeImage != null));
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, 0); // 初期透明
                                                        // フェードイン
                for (float alpha = 0f; alpha <= 1f; alpha += Time.deltaTime * 0.5f)
                {
                    fadeImage.color = new Color(0, 0, 0, alpha);
                    yield return null;
                }
                fadeImage.color = new Color(0, 0, 0, 1f); // 完全な黒
                yield return new WaitForSeconds(3.5f); // 3.5秒待機
                                                        // フェードアウト
                for (float alpha = 1f; alpha >= 0f; alpha -= Time.deltaTime * 0.5f)
                {
                    fadeImage.color = new Color(0, 0, 0, alpha);
                    yield return null;
                }
                fadeImage.color = new Color(0, 0, 0, 0); // 完全な透明
                fadePanel.SetActive(false); // 表示終了後に非アクティブ
            }
            else
            {
                Debug.LogError("ShowBlackScreen - FadeImage component not found on FadePanel");
            }
        }
        else
        {
            Debug.LogError("ShowBlackScreen - FadePanel not assigned");
        }
    }

    System.Collections.IEnumerator FadeToMainStory()
    {
        UpdateCharacterImage(null, null); // 遷移前に透明化
        Debug.Log("FadeToMainStory - FadePanel assigned: " + (fadePanel != null));
        if (fadePanel != null)
        {
            fadePanel.SetActive(true); // 強制的にアクティブ化
            Image fadeImage = fadePanel.GetComponent<Image>();
            Debug.Log("FadeToMainStory - FadeImage found: " + (fadeImage != null));
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, 0); // 初期透明
                                                        // フェードイン
                for (float alpha = 0f; alpha <= 1f; alpha += Time.deltaTime)
                {
                    fadeImage.color = new Color(0, 0, 0, alpha);
                    Debug.Log("FadeToMainStory - Fading in, alpha: " + alpha); // 進行状況ログ
                    yield return null;
                }
                fadeImage.color = new Color(0, 0, 0, 1f); // 完全な黒
                Debug.Log("FadeToMainStory - Fade in complete");
                yield return new WaitForSeconds(0.1f); // フェードイン後の待機（調整可能）
            }
            else
            {
                Debug.LogError("FadeToMainStory - FadeImage component not found on FadePanel");
            }
        }
        else
        {
            Debug.LogError("FadeToMainStory - FadePanel not assigned");
        }

        // シーン遷移（フェード後に実行）
        SceneManager.LoadScene("MainScene");

        if (fadePanel != null)
        {
            Image fadeImage = fadePanel.GetComponent<Image>();
            if (fadeImage != null)
            {
                // フェードアウト
                for (float alpha = 1f; alpha >= 0f; alpha -= Time.deltaTime)
                {
                    fadeImage.color = new Color(0, 0, 0, alpha);
                    Debug.Log("FadeToMainStory - Fading out, alpha: " + alpha); // 進行状況ログ
                    yield return null;
                }
                fadeImage.color = new Color(0, 0, 0, 0f); // 完全な透明
                fadePanel.SetActive(false); // フェード終了後に非アクティブ
                Debug.Log("FadeToMainStory - Fade out complete");
            }
        }

        yield return new WaitForEndOfFrame();
        if (isFromGateStory)
        {
            StartCoroutine(PlayMainStory());
            hasPlayedMainStory = true; // メインシーン到達時にフラグを設定
        }
    }

    System.Collections.IEnumerator PlayMainStory()
    {
        characterImageRight.SetActive(true);
        if (characterImageLeft != null) characterImageLeft.SetActive(true); // Activate left image for this story

        for (int i = 0; i < mainLines.Length; i++)
        {
            dialogBox.SetActive(true);
            dialogText.text = mainLines[i];
            UpdateCharacterImage(null, null); // Clear both images

            if (i == 1 || i == 4 || i == 5) // Female student speaks
            {
                UpdateCharacterImage(femaleStudentRelieved, protagonistNormal); // Female student on left
            }
            else if (i == 2 || i == 3) // Assuming other characters might be on right or no image
            {
                // Example: If you want Protagonist on the right during these lines:
                // UpdateCharacterImage(protagonistNormal, null); 
                UpdateCharacterImage(null, null); // No image needed, or add other character sprites
            }
            else if (i == 0)
            {
                UpdateCharacterImage(null, null); // Narrator
            }
            else
            {
                UpdateCharacterImage(null, null);
            }
            yield return new WaitForSeconds(3f);
        }
        dialogBox.SetActive(false);
        characterImageRight.SetActive(false);
        if (characterImageLeft != null) characterImageLeft.SetActive(false);
        if (mainTrigger != null) // MainTriggerのコライダーを無効化
        {
            mainTrigger.enabled = false;
            Debug.Log("MainTrigger disabled after PlayMainStory");
        }
        hasPlayedMainStory = true; // ここでフラグを設定
    }

    // キャラクタ画像の更新と透明化を管理
    // spriteLeft: 左側のキャラクターに表示するスプライト
    // spriteRight: 右側のキャラクターに表示するスプライト
    private void UpdateCharacterImage(Sprite spriteLeft, Sprite spriteRight)
    {
        Image imageComponentLeft = null;
        if (characterImageLeft != null)
        {
            imageComponentLeft = characterImageLeft.GetComponent<Image>();
        }

        Image imageComponentRight = null;
        if (characterImageRight != null)
        {
            imageComponentRight = characterImageRight.GetComponent<Image>();
        }

        if (imageComponentLeft != null)
        {
            imageComponentLeft.sprite = spriteLeft;
            imageComponentLeft.color = spriteLeft != null ? Color.white : new Color(1, 1, 1, 0);
        }
        if (imageComponentRight != null)
        {
            imageComponentRight.sprite = spriteRight;
            imageComponentRight.color = spriteRight != null ? Color.white : new Color(1, 1, 1, 0);
        }
    }
}