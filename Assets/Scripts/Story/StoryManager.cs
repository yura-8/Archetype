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
        "�]�Z���Ă����@�͊w�Z�Ɍ������Ă����B",
        "�����́u�@�������w�Z�v�ł���B",
        "�@�͓��i���̊w�Z�ɍs�������킯�ł��Ȃ��������A�T�C�{�[�O�ł��鎩�g�ɂ��ʂ��₷�����x�������Ă��āA�����߂��Ƃ������R�ł����ɂ������Ƃ����߂��B",
        "�����V�C���ƓۋC�Ȏ����l���Ȃ�������Ă���Ɖ���瑛�����������������Ă���B",
        "�u�Ȃ񂾂��Z��̕������������ȁB�s���Ă݂悤�B�v"
    };
    private string[] gateLines = {
        "���̂�������֍s���Ă݂�Ǝ����悤�Ȑ����𒅂����q���k�Ɣ��߂𒅂����m��ʒj�����������Ă���B",
        "�u���{�b�g�͓���ł͂���܂���I���������̃��{�b�g��Ԃ��Ă��������I�v",
        "���q���k�͒j���ɂݕt����B",
        "�u�������ɉ�ꂩ���Ă���ʎY�^���{�b�g����Ȃ����B���̉����L�����p���Ă��ƌ����Ă���̂�����A�J�늴�ӂ̌��t���q�ׂ�ׂ����낤�H�v",
        "�u�ʎY�^�Ƃ��֌W����܂���I�����郍�{�b�g���킴�킴�󂷕K�v�Ȃ�Ė�������Ȃ��ł����I�v",
        "�u����قǂ̃o�b�e���[�Ȃ�΃��c�̓��͂Ɏg���������B�����܂ŉ]���̂Ȃ�ΊO���͌N�ɂ����Ă��ǂ��B�v",
        "���������ƒj�̓|�P�b�g����h���C�o�[�����o�����B",
        "�u��߂āI�v",
        "�u���̃��{�b�g���󂻂����Ă����Ȃ�A��������ɂȂ��Ă��I�v",
        "�u�Ȃ񂾂��O�́H������������ɓn���B�v",
        "�u�f��B�v",
        "�u�ʓ|���ȁc�v",
        "�j�̓X�}�z�����o�����B",
        "�������͂���Ɖ�������Ƃ��Ȃ������̃��{�b�g�������Ƙ@�����͂ށB",
        "�u��邵���Ȃ��I�v",
        "�@�̓J�o�����珬���Ȕ������o����."
    };
    private string[] mainLines = {
        "�[�Ó]�[",
        "�ŏ��̎��Ƃ��I���A����ꂽ�ʂ�̋����ɍs���ƁA�����ɒւ͂����B",
        "�u�悤�����I���ꂪ���̏������镔���AG.E.A.R.�ł��I�v",
        "�����ĕ�����2�l���Љ�ꂽ�B",
        "���C�ȓ������A���C�Ȍ�y���B",
        "�u���̕����͖\���������{�b�g���~�߂鎖����ȋƖ��ł��B�v",
        "�u�����ł������̕ӂ�̖\���������{�b�g�𐧈����ɍs���܂��傤�I�v"
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