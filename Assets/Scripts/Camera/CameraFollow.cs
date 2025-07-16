using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // �Ǐ]�Ώہi�v���C���[�j
    [SerializeField] private float smoothSpeed = 0.125f; // ���炩���i0�`1�j
    [SerializeField] private Vector3 offset; // �J�����ƃv���C���[�̃I�t�Z�b�g
    [SerializeField] private float minOrthographicSize = 5f; // �ŏ��X�P�[��
    [SerializeField] private float maxOrthographicSize = 10f; // �ő�X�P�[��

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (target == null)
        {
            // �f�t�H���g�Ńv���C���[��������
            target = GameObject.FindWithTag("Player")?.transform;
        }
        // �����I�t�Z�b�g��ݒ�
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // �ڕW�ʒu�Ɋ��炩�Ɉړ�
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);

        // �J�����̃X�P�[���𒲐��i�I�v�V�����j
        AdjustOrthographicSize();
    }

    void AdjustOrthographicSize()
    {
        if (cam.orthographic)
        {
            float targetSize = Mathf.Lerp(minOrthographicSize, maxOrthographicSize, 0.5f); // �����œ��I�ɒ����\
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, smoothSpeed);
        }
    }

    // �C���X�y�N�^�[�Ń^�[�Q�b�g��ݒ�i�I�v�V�����j
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}