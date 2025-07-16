using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // �ړ����x
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // ���E���́iA/D�L�[�j
        float moveY = Input.GetAxisRaw("Vertical");   // �㉺���́iW/S�L�[�j
        Vector2 movement = new Vector2(moveX, moveY).normalized * speed;
        rb.linearVelocity = movement; // �ړ�
    }
}
