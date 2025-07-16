using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private float encounterChance = 0.05f; // �G���J�E���g�m���i5%�j
    [SerializeField] private LayerMask encounterAreaLayer; // �G���J�E���g�G���A�̃��C���[
    private float encounterTimer; // �G���J�E���g����p�^�C�}�[
    private const float CHECK_INTERVAL = 1f; // 1�b���Ƃɔ���

    void Update()
    {
        CheckEncounter();
    }

    void CheckEncounter()
    {
        // �v���C���[�̈ʒu�ŃI�[�o�[���b�v�`�F�b�N
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, encounterAreaLayer);
        if (hitColliders.Length > 0) // �G���J�E���g�G���A���ɓ�����
        {
            encounterTimer += Time.deltaTime;
            if (encounterTimer >= CHECK_INTERVAL)
            {
                encounterTimer = 0f;
                // ���m���ŃG���J�E���g
                if (Random.value <= encounterChance)
                {
                    TriggerBattle();
                }
            }
        }
        else
        {
            encounterTimer = 0f; // �G���A�O�Ȃ烊�Z�b�g
        }
    }

    void TriggerBattle()
    {
        // BattleScene �ɑJ��
        SceneManager.LoadScene("BattleScene"); // BattleScene �̃V�[�����ɕύX
        // �K�v�ɉ����Đ퓬�f�[�^��ۑ��i��F�G�̎�ށj
        // PlayerPrefs ��ÓI�N���X���g�p�\
    }

    // �f�o�b�O�p: �G���J�E���g�G���A�̉���
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}