using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro�p

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startText; // Inspector��TextMeshPro���A�^�b�`
    private float blinkTimer = 0f;
    private float blinkInterval = 0.8f; // �_�ŊԊu�i�b�j

    void Update()
    {
        // Z�L�[�ŃV�[���J��
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SceneManager.LoadScene("Scene1");
        }

        // �_�ŏ���
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            startText.enabled = !startText.enabled; // �\��/��\����؂�ւ�
            blinkTimer = 0f;
        }
    }
}