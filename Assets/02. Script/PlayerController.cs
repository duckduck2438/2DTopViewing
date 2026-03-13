using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float h, v;
    Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // GetAxisRaw는 -1, 0, 1의 값을 반환한다. GetAxis는 -1과 1 사이의 값을 반환한다.
        // 따라서 현재 만드는 쯔꾸르형식의 게임에서는 해당 함수가 적절하다.
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(h, v) * 5f;
    }
}
