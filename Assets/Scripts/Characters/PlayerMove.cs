using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // 移動速度
    private Rigidbody2D rb;

    // 静的変数で位置をシーン間で保持
    private static Vector3 lastPlayerPosition = Vector3.zero;
    private static Vector3 lastCameraPosition = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // MainSceneに戻った際に前回の位置に復元
        if (SceneManager.GetActiveScene().name == "MainScene" && lastPlayerPosition != Vector3.zero)
        {
            transform.position = lastPlayerPosition;
            Debug.Log("Player position restored: " + lastPlayerPosition);

            // カメラの位置を復元
            CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
            if (cameraFollow != null && lastCameraPosition != Vector3.zero)
            {
                cameraFollow.transform.position = new Vector3(lastCameraPosition.x, lastCameraPosition.y, cameraFollow.transform.position.z);
                Debug.Log("Camera position restored: " + lastCameraPosition);
            }
        }
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // 左右入力 (A/Dキー)
        float moveY = Input.GetAxisRaw("Vertical");   // 上下入力 (W/Sキー)
        Vector2 movement = new Vector2(moveX, moveY).normalized * speed;
        rb.linearVelocity = movement; // 移動
    }

    // シーン遷移前に位置を保存するメソッド
    public void SavePosition()
    {
        lastPlayerPosition = transform.position;
        Debug.Log("Player position saved: " + lastPlayerPosition);

        // カメラの位置を保存
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            lastCameraPosition = cameraFollow.transform.position;
            Debug.Log("Camera position saved: " + lastCameraPosition);
        }
    }

    // 位置をリセットするメソッド（オプション）
    public void ResetPosition()
    {
        lastPlayerPosition = Vector3.zero;
        lastCameraPosition = Vector3.zero;
    }
}