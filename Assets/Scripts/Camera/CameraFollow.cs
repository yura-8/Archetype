using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // 追従対象（プレイヤー）
    [SerializeField] private float smoothSpeed = 0.125f; // 滑らかさ（0～1）
    [SerializeField] private Vector3 offset; // カメラとプレイヤーのオフセット
    [SerializeField] private float minOrthographicSize = 5f; // 最小スケール
    [SerializeField] private float maxOrthographicSize = 10f; // 最大スケール

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (target == null)
        {
            // デフォルトでプレイヤーを見つける
            target = GameObject.FindWithTag("Player")?.transform;
        }
        // 初期オフセットを設定
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 目標位置に滑らかに移動
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);

        // カメラのスケールを調整（オプション）
        AdjustOrthographicSize();
    }

    void AdjustOrthographicSize()
    {
        if (cam.orthographic)
        {
            float targetSize = Mathf.Lerp(minOrthographicSize, maxOrthographicSize, 0.5f); // ここで動的に調整可能
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, smoothSpeed);
        }
    }

    // インスペクターでターゲットを設定（オプション）
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}