using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private GameObject characterImage;
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Sprite protagonistNormal, protagonistSerious, protagonistSurprised, protagonistSmile;
    [SerializeField] private Sprite femaleStudentNormal, femaleStudentSad, femaleStudentAngry, femaleStudentRelieved;
    [SerializeField] private Sprite whiteCoatManNormal, whiteCoatManSmug, whiteCoatManAngry;
    private bool isPlayerControlEnabled = false;
    private bool isGateTriggerDisabled = false;
    private string[] introLines = {
        "転校してきた蓮は学校に向かっていた。",
        "そこは「機星高等学校」である。",
        "蓮は特段この学校に行きたいわけでもなかったが、サイボーグである自身にも通いやすい制度が整っていて、何より近いという理由でここにいくことを決めた。",
        "いい天気だと呑気な事を考えながら歩いていると何やら騒がしい声が聞こえてくる。",
        "「なんだか校門の方が騒がしいな。行ってみよう。」"
    };
    private string[] gateLines = {
        "声のする方向へ行ってみると似たような制服を着た女子生徒と白衣を着た見知らぬ男が言い争っている。",
        "「ロボットは道具ではありません！今すぐそのロボットを返してください！」",
        "女子生徒は男を睨み付ける。",
        "「もう既に壊れかけている量産型ロボットじゃないか。この俺が有効活用してやると言っているのだから、寧ろ感謝の言葉を述べるべきだろう？」",
        "「量産型とか関係ありません！直せるロボットをわざわざ壊す必要なんて無いじゃないですか！」",
        "「これほどのバッテリーならばヤツの動力に使えそうだ。そこまで云うのならば外装は君にあげても良い。」",
        "そう言うと男はポケットからドライバーを取り出した。",
        "「やめて！」",
        "「そのロボットを壊そうっていうなら、俺が相手になってやる！」",
        "「なんだお前は？それをこっちに渡せ。」",
        "「断る。」",
        "「面倒だな…」",
        "男はスマホを取り出した。",
        "何やら入力すると何処からともなく複数のロボットが少女と蓮を取り囲む。",
        "「やるしかない！」",
        "蓮はカバンから小さな箱を取り出した."
    };
    private string[] mainLines = {
        "ー暗転ー",
        "最初の授業が終わり、言われた通りの教室に行くと、そこに椿はいた。",
        "「ようこそ！これが私の所属する部活、G.E.A.R.です！」",
        "そして部員の2人を紹介された。",
        "元気な同級生、強気な後輩だ。",
        "「この部活は暴走したロボットを止める事が主な業務です。」",
        "「早速ですがこの辺りの暴走したロボットを制圧しに行きましょう！」"
    };

    void Start()
    {
        characterImage.SetActive(false);
        dialogBox.SetActive(false);
        isGateTriggerDisabled = false;
        StartCoroutine(PlayIntroStory());
    }

    System.Collections.IEnumerator PlayIntroStory()
    {
        isPlayerControlEnabled = false;
        Debug.Log("PlayIntroStory Started, Control: " + isPlayerControlEnabled);
        for (int i = 0; i < introLines.Length; i++)
        {
            dialogBox.SetActive(true);
            dialogText.text = introLines[i];
            yield return new WaitForSeconds(2f);
        }
        dialogBox.SetActive(false);
        isPlayerControlEnabled = true;
        Debug.Log("PlayIntroStory Ended, Control: " + isPlayerControlEnabled);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Entered: " + other.name + ", Tag: " + other.tag + ", Enabled: " + isPlayerControlEnabled + ", Disabled: " + isGateTriggerDisabled);
        if (other.CompareTag("Player") && isPlayerControlEnabled && !isGateTriggerDisabled)
        {
            StartCoroutine(PlayGateStory());
        }
    }

    System.Collections.IEnumerator PlayGateStory()
    {
        isPlayerControlEnabled = false;
        Debug.Log("PlayGateStory Started, Control: " + isPlayerControlEnabled);
        characterImage.SetActive(true);
        for (int i = 0; i < gateLines.Length; i++)
        {
            dialogBox.SetActive(true);
            dialogText.text = gateLines[i];
            if (i == 1 || i == 4 || i == 7) characterImage.GetComponent<UnityEngine.UI.Image>().sprite = femaleStudentAngry;
            else if (i == 3 || i == 5) characterImage.GetComponent<UnityEngine.UI.Image>().sprite = whiteCoatManSmug;
            else if (i == 9 || i == 10) characterImage.GetComponent<UnityEngine.UI.Image>().sprite = whiteCoatManAngry;
            else if (i == 8) characterImage.GetComponent<UnityEngine.UI.Image>().sprite = protagonistSerious;
            else if (i == 14) characterImage.GetComponent<UnityEngine.UI.Image>().sprite = protagonistSurprised;
            else characterImage.GetComponent<UnityEngine.UI.Image>().sprite = null;
            yield return new WaitForSeconds(2f);
            if (i == 15)
            {
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene("BattleScene");
            }
        }
        dialogBox.SetActive(false);
        characterImage.SetActive(false);
        isPlayerControlEnabled = true;
        Debug.Log("PlayGateStory Ended, Control: " + isPlayerControlEnabled);
        isGateTriggerDisabled = true;
    }

    public void OnBattleEnd()
    {
        StartCoroutine(PlayMainStory());
    }

    System.Collections.IEnumerator PlayMainStory()
    {
        isPlayerControlEnabled = false;
        Debug.Log("PlayMainStory Started, Control: " + isPlayerControlEnabled);
        SceneManager.LoadScene("MainScene");
        yield return new WaitForSeconds(1f);
        characterImage.SetActive(true);
        for (int i = 0; i < mainLines.Length; i++)
        {
            dialogBox.SetActive(true);
            dialogText.text = mainLines[i];
            if (i == 2) characterImage.GetComponent<UnityEngine.UI.Image>().sprite = femaleStudentRelieved;
            else if (i == 5) characterImage.GetComponent<UnityEngine.UI.Image>().sprite = femaleStudentAngry;
            else if (i == 6) characterImage.GetComponent<UnityEngine.UI.Image>().sprite = femaleStudentRelieved;
            else characterImage.GetComponent<UnityEngine.UI.Image>().sprite = null;
            yield return new WaitForSeconds(2f);
        }
        dialogBox.SetActive(false);
        characterImage.SetActive(false);
        isPlayerControlEnabled = true;
        Debug.Log("PlayMainStory Ended, Control: " + isPlayerControlEnabled);
    }

    void Update()
    {
        if (isPlayerControlEnabled)
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                    animator.SetInteger("Direction", 1);
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    animator.SetInteger("Direction", 0);
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                    animator.SetInteger("Direction", 2);
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                    animator.SetInteger("Direction", 3);
            }
        }
    }
}